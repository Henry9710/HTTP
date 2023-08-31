using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace HttpWebRequestQueueWithResume
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxConcurrency = 3;
            int total = 6;
            
            BlockingCollection<string> requestQueue = new BlockingCollection<string>();
            //AutoResetEvent autoResetEvent = new AutoResetEvent(true);

            Thread producer = new Thread(() =>
            {
                for (int i = 1; i <= total; i++)
                {
                    requestQueue.Add($"https://static.runoob.com/images/demo/demo{i}.jpg");
                }
                requestQueue.CompleteAdding(); 
            });
            
            Thread[] consumers = new Thread[maxConcurrency];
            for (int i = 0; i < maxConcurrency; i++)
            {
                consumers[i] = new Thread(() =>
                {
                    foreach (string url in requestQueue.GetConsumingEnumerable())
                    {
                        ProcessRequest(url);
                    }
                });
            }
            
            producer.Start();
            foreach (Thread consumer in consumers)
            {
                consumer.Start();
            }
            
            producer.Join();
            foreach (Thread consumer in consumers)
            {
                consumer.Join();
            }

            Console.WriteLine("All downloads completed.");
        }

        static void ProcessRequest(string url)
        {
            string fileName = Path.GetFileName(url);
            long resumePosition = 0;

            if (File.Exists(fileName))
            {
                FileInfo fileInfo = new FileInfo(fileName);
                resumePosition = fileInfo.Length;
            }

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AddRange(resumePosition);

                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (FileStream fileStream = new FileStream(fileName, FileMode.Append))
                {
                    byte[] buffer = new byte[10240];
                    int bytesRead;

                    while ((bytesRead = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                        resumePosition += bytesRead;

                        double progressPercentage = (double)resumePosition / response.ContentLength * 100;
                        Console.WriteLine(
                            $"Download Progress for {fileName}: {(int)progressPercentage}% ({resumePosition}/{response.ContentLength} bytes)");
                    }

                    Console.WriteLine($"HTTP Response Headers for {fileName}:");
                    foreach (string headerName in response.Headers.AllKeys)
                    {
                        Console.WriteLine($"{headerName}: {response.Headers[headerName]}");
                    }

                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        string responseBody = reader.ReadToEnd();
                        Console.WriteLine($"HTTP Response Body for {fileName}:\n{responseBody}");
                    }
                }

                Console.WriteLine($"Download completed for {fileName}.");
            }
            catch (WebException ex)
            {
                Console.WriteLine($"WebException for {fileName}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception for {fileName}: {ex.Message}");
            }
        }
    }
}
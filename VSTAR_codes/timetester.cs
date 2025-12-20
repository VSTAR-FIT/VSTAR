using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading; // Required for Thread.Sleep

class Program
{
    static void Main(string[] args)
    {
        // 1. Create and start the Stopwatch
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        // 2. Place the code you want to measure here
        
        Vector3 tau = new Vector3(1, 1, 1); // example external torque
        double[] rot = new double[] { 0, 0, 0, 1, 0, 0, 0 }; // example initial state vector
        RotateOrion3 Spin = new RotateOrion3();
        rot = Spin.Rotate3(rot, tau);

        // 3. Stop the timer
        stopWatch.Stop();

        // 4. Get the elapsed time as a TimeSpan value
        TimeSpan ts = stopWatch.Elapsed;

        // 5. Format and display the elapsed time
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);
        Console.WriteLine("Elapsed milliseconds: " + stopWatch.ElapsedMilliseconds);
    }
}

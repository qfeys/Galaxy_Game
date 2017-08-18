using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using Assets.Scripts.Empires;
using Assets.Scripts.Data;

namespace Assets.Scripts.Simulation
{
    public class God : MonoBehaviour
    {
        static God TheOne;
        internal static Empire PlayerEmpire { get; private set; }
        public static readonly Dictionary<string, TimeSpan> timeSteps = new Dictionary<string, TimeSpan>() {    { "1s", TimeSpan.FromSeconds(1) },
                                                                                                                { "5s", TimeSpan.FromSeconds(5) },
                                                                                                                { "20s", TimeSpan.FromSeconds(30) },
                                                                                                                { "1m", TimeSpan.FromMinutes(1) },
                                                                                                                { "5m", TimeSpan.FromMinutes(5) },
                                                                                                                { "20m", TimeSpan.FromMinutes(20) },
                                                                                                                { "1h", TimeSpan.FromHours(1) },
                                                                                                                { "4h", TimeSpan.FromHours(4) },
                                                                                                                { "12h", TimeSpan.FromHours(12) },
                                                                                                                { "1d", TimeSpan.FromDays(1) },
                                                                                                                { "5d", TimeSpan.FromDays(5) },
                                                                                                                { "30d", TimeSpan.FromDays(30) } };

        public static DateTime Time { get; internal set; }
        public static TimeSpan deltaTime;
        Thread simThread;
        static Exception mainThreadException;
        static bool abort = false;
        static bool nextRealTimeTickReady = false;
        static bool nextSimTimeTickReady = false;
        static bool isPaused = false;
        const float REAL_TIME_BETWEEN_TICKS = 1;
        float realTimeSindsLastTick;
        

        // Use this for initialization
        void Start()
        {
            if (TheOne != null) throw new Exception("A second god is created");
            TheOne = this;
            Time = new DateTime(2100, 1, 1);

            Init();


            deltaTime = TimeSpan.FromSeconds(1);
            simThread = new Thread(()=> { try { RunTime(); } catch (Exception e) { mainThreadException = e; } });
            simThread.Start();
        }

        // Update is called once per frame
        void Update()
        {
            ExcicuteQueuedActions();
            if(isPaused == false)
                realTimeSindsLastTick += UnityEngine.Time.deltaTime;
            if(realTimeSindsLastTick > REAL_TIME_BETWEEN_TICKS)
            {
                if (nextSimTimeTickReady)
                {
                    // Debug.Log("Next real time tick:"+ Time.ToString("yyyy.MM.dd HH:mm:ss"));
                    nextRealTimeTickReady = true;
                    realTimeSindsLastTick = 0;
                }
                else Debug.Log("waiting on simulation. RTsLT: "+realTimeSindsLastTick);
            }
            if (mainThreadException != null)
                Debug.LogError( mainThreadException);
        }

        internal static void Pause()
        {
            isPaused = !isPaused;
        }

        public static void Init()
        {
            Debug.Log("Reading mod files at " + System.IO.Directory.GetCurrentDirectory());
            ModParser.ParseAllFiles();
            Data.Graphics.LoadGraphics();
            Localisation.Load();

            Debug.Log("Initialising galaxy");
            Bodies.Galaxy.Create(1, 21);

            Debug.Log("Initialising Empires");
            //PlayerEmpire = new Empire("TyroTech Empire", Bodies.Galaxy.systems[0].RandLivableWorld());
            PlayerEmpire = new Empire("TyroTech Empire", Bodies.Galaxy.systems[0].Planets[0]);

            Rendering.DisplayManager.TheOne.Init();
            Rendering.DisplayManager.TheOne.DisplaySystem(Bodies.Galaxy.systems[0]);
        }

        private static void RunTime()
        {
            while (abort == false)
            {
                if (EventSchedule.NextEvent == DateTime.MaxValue)
                {
                    Log("no future event");
                    abort = true;
                    break;
                }
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                DateTime endOfTick = Time + deltaTime;
                DateTime maxDate = endOfTick;
                EventSchedule.CanProgress(endOfTick, out maxDate);
                int loops = EventSchedule.Progress(maxDate);
                Time = maxDate;
                
                nextSimTimeTickReady = true;
                sw.Stop();
                long mils = sw.ElapsedMilliseconds;
                // ExcicuteOnUnityThread(() => Debug.Log("Main loop processed " + loops + " events in " + mils + " ms"));
                while (nextRealTimeTickReady == false  && abort != true)
                {
                    Thread.Sleep(100);
                }
                nextSimTimeTickReady = false;
                nextRealTimeTickReady = false;
            }
            ExcicuteOnUnityThread(() => Debug.Log("run time aborted"));
        }

        public void OnDestroy()
        {
            abort = true;
            simThread.Abort();
            Debug.Log("Secondary thread: " + simThread.ThreadState);
        }

        static Queue<Action> unityActions = new Queue<Action>();
        public static void ExcicuteOnUnityThread(Action a)
        {
            unityActions.Enqueue(a);
        }

        public static void Log(string message)
        {
            ExcicuteOnUnityThread(() => Debug.Log(message));
        }

        void ExcicuteQueuedActions()
        {
            while(unityActions.Count != 0)
            {
                unityActions.Dequeue().Invoke();
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using Assets.Scripts.Empires;

namespace Assets.Scripts.Simulation
{
    public class God : MonoBehaviour
    {
        static God TheOne;
        internal static Empires.Empire PlayerEmpire { get; private set; }
        public static readonly Dictionary<string, TimeSpan> TimeSteps = new Dictionary<string, TimeSpan>() {    { "1s", TimeSpan.FromSeconds(1) },
                                                                                                                { "5s", TimeSpan.FromSeconds(5) },
                                                                                                                { "20s", TimeSpan.FromSeconds(30) },
                                                                                                                { "1m", TimeSpan.FromMinutes(1) },
                                                                                                                { "5m", TimeSpan.FromMinutes(5) },
                                                                                                                { "20m", TimeSpan.FromMinutes(1) },
                                                                                                                { "1h", TimeSpan.FromHours(1) },
                                                                                                                { "4h", TimeSpan.FromHours(4) },
                                                                                                                { "12h", TimeSpan.FromHours(12) },
                                                                                                                { "1d", TimeSpan.FromDays(1) },
                                                                                                                { "5d", TimeSpan.FromDays(5) },
                                                                                                                { "30d", TimeSpan.FromDays(30) } };

        public static DateTime Time { get; internal set; }
        public static TimeSpan DeltaTime;
        Thread mainThread;
        static Exception mainThreadException;
        static bool abort = false;
        static bool NextRealTimeTickReady = false;
        static bool NextSimTimeTickReady = false;
        const float REAL_TIME_BETWEEN_TICKS = 1;
        float realTimeSindsLastTick;
        

        // Use this for initialization
        void Start()
        {
            if (TheOne != null) throw new Exception("A second god is created");
            TheOne = this;
            Time = new DateTime(2100, 1, 1);

            Init();

            Rendering.DisplayManager.TheOne.DisplaySystem((Bodies.StarSystem)Bodies.Core.instance.Childeren[0]);

            DeltaTime = TimeSpan.FromSeconds(1);
            mainThread = new Thread(()=> { try { RunTime(); } catch (Exception e) { mainThreadException = e; } });
            mainThread.Start();
        }

        // Update is called once per frame
        void Update()
        {
            ExcicuteQueuedActions();
            realTimeSindsLastTick += UnityEngine.Time.deltaTime;
            if(realTimeSindsLastTick > REAL_TIME_BETWEEN_TICKS)
            {
                if (NextSimTimeTickReady)
                {
                    Debug.Log("Next real time tick:"+ Time.ToString("yyyy.MM.dd HH:mm:ss"));
                    NextRealTimeTickReady = true;
                    realTimeSindsLastTick = 0;
                }
                else Debug.Log("waiting on simulation");
            }
            if (mainThreadException != null)
                throw mainThreadException;
        }

        public static void Init()
        {
            Debug.Log(System.IO.Directory.GetCurrentDirectory());
            Debug.Log("Initialising Technologies");
            Empires.Technology.Academy.Init();
            Debug.Log("Initialising galaxy");
            Bodies.Core.Create(1, 22);

            Debug.Log("Initialising Empires");
            PlayerEmpire = new Empire("TyroTech Empire", ((Bodies.StarSystem)Bodies.Core.instance.Childeren[0]).RandLivableWorld());
        }

        private static void RunTime()
        {
            while(abort == false)
            {
                if(EventSchedule.nextEvent == DateTime.MaxValue)
                {
                    abort = true;
                    break;
                }
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                int loops = 0;
                DateTime endOfTick = Time + DeltaTime;
                while(EventSchedule.nextEvent < endOfTick)
                {
                    loops++;
                    Time = EventSchedule.nextEvent;
                    bool interupt = false;
                    EventSchedule.ProcessNextEvent(out interupt);
                    if (interupt == true)
                        break;
                }
                NextSimTimeTickReady = true;
                sw.Stop();
                long mils = sw.ElapsedMilliseconds;
                ExcicuteOnUnityThread(() => Debug.Log("Main loop processed " + loops + " events in " + mils + " ms"));
                while (NextRealTimeTickReady == false && abort != true)
                {
                    Thread.Sleep(100);
                }
                NextSimTimeTickReady = false;
                NextRealTimeTickReady = false;
            }
        }

        public void OnDestroy()
        {
            abort = true;
            mainThread.Abort();
            Debug.Log("Secondary thread: " + mainThread.ThreadState);
        }

        static Queue<Action> unityActions = new Queue<Action>();
        public static void ExcicuteOnUnityThread(Action a)
        {
            unityActions.Enqueue(a);
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

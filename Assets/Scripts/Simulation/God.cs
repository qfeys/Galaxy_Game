using UnityEngine;
using System.Collections.Generic;
using System;
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

        // Use this for initialization
        void Start()
        {
            if (TheOne != null) throw new Exception("A second god is created");
            TheOne = this;
            Time = new DateTime(2100, 1, 1);

            Init();

            Rendering.DisplayManager.TheOne.DisplaySystem((Bodies.StarSystem)Bodies.Core.instance.Childeren[0]);
        }

        // Update is called once per frame
        void Update()
        {

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
    }
}

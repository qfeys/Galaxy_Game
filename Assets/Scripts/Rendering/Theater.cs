using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Rendering
{
    class Theater
    {
        /// <summary>
        /// log scale - high values are zoomed in
        /// </summary>
        public float zoom = 0.0f;
        public float Scale { get { return Mathf.Pow(10, -zoom); } }
        public Vector3 Center { get; private set; }
        private Vector2 camRot = Vector2.zero;
        /// <summary>
        /// Camera rotation in rad
        /// </summary>
        public Vector2 CamRot { get { return camRot; } set { camRot = value; } }
        bool tilt = false;
        Action render;

        public Theater(Action render, bool tilt = false)
        {
            this.render = render;
            this.tilt = tilt;
        }

        public void Render() { render(); }

        public void SetCenter(Vector3 c) { Center = c; }
        public void SetCenter(Bodies.Planet p)
        {
            VectorS posS = p.OrbElements.GetPositionSphere(Simulation.God.Time);
            Vector3 posPar = p.ParentPlanet == null ?
                p.Parent.OrbElements.GetPosition(Simulation.God.Time) : p.ParentPlanet.OrbElements.GetPosition(Simulation.God.Time);
            Vector3 posTrue = (Vector3)posS + posPar;
            SetCenter(posTrue);
        }

        public void MoveCenter(Vector2 v) { Center += (Vector3)v / Mathf.Pow(10, -zoom) * 0.1f; }

        public void ResetView() { zoom = 0; Center = Vector2.zero; camRot = Vector2.zero; PlaceCamera(); }

        /// <summary>
        /// Tilts the camera so it conferms to the theater state
        /// </summary>
        public void PlaceCamera()
        {
            if (tilt == false)
            {
                float x = 40 * Mathf.Sin(camRot.x) * Mathf.Cos(camRot.y);
                float y = 40 * Mathf.Sin(camRot.y);
                float z = -40 * Mathf.Cos(camRot.x) * Mathf.Cos(camRot.y);
                Camera.main.transform.position = new Vector3(x, y, z);
                Camera.main.transform.rotation = Quaternion.Euler(camRot.y * Mathf.Rad2Deg, -camRot.x * Mathf.Rad2Deg, 0);
            }
            else
            {
                float x = 40 * Mathf.Sin(camRot.x) * Mathf.Cos(camRot.y);
                float y = -40 * Mathf.Cos(camRot.x) * Mathf.Cos(-camRot.y);
                float z = -40 * Mathf.Sin(camRot.y);
                Camera.main.transform.position = new Vector3(x, y, z);
                float a = camRot.x * Mathf.Rad2Deg - camRot.y * Mathf.Rad2Deg * Mathf.Sin(camRot.x);
                Camera.main.transform.rotation = Quaternion.AngleAxis(camRot.x * Mathf.Rad2Deg, Vector3.forward) * Quaternion.AngleAxis(camRot.y * Mathf.Rad2Deg - 90, Vector3.right);
            }
        }
    }
}

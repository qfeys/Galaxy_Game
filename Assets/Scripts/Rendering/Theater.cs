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

        public float zoom = 0.0f; // log scale - high values are zoomed in
        public Vector3 center { get; private set; }
        private Vector2 camRot = Vector2.zero;
        public Vector2 CamRot { get { return camRot; } set { camRot = value; Debug.Log(value); } }
        Action render;

        public Theater (Action render)
        {
            this.render = render;
        }

        public void Render() { render(); }

        public void SetCenter(Vector3 c) { center = c; }
        public void SetCenter(Bodies.Planet p)
        {
            VectorS posS = p.OrbElements.GetPositionSphere(Simulation.God.Time);
            Vector3 posPar = p.ParentPlanet == null ?
                p.Parent.OrbElements.GetPosition(Simulation.God.Time) : p.ParentPlanet.OrbElements.GetPosition(Simulation.God.Time);
            Vector3 posTrue = (Vector3)posS + posPar;
            SetCenter(posTrue);
        }

        public void MoveCenter(Vector2 v) { center += (Vector3)v / Mathf.Pow(10, -zoom) * 0.1f; }

        public void ResetView() { zoom = 0; center = Vector2.zero; camRot = Vector2.zero; PlaceCamera(); }

        /// <summary>
        /// Tilts the camera so it conferms to the theater state
        /// </summary>
        public void PlaceCamera()
        {
            float x = 40 * Mathf.Sin(camRot.x) * Mathf.Cos(camRot.y);
            float y = 40 * Mathf.Sin(camRot.y);
            float z = -40 * Mathf.Cos(camRot.x) * Mathf.Cos(camRot.y);
            Camera.main.transform.position = new Vector3(x, y, z);
            Camera.main.transform.rotation = Quaternion.Euler(camRot.y * Mathf.Rad2Deg, -camRot.x * Mathf.Rad2Deg, 0);
        }
    }
}

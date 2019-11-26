// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 800.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;


        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            //gl.Enable(OpenGL.GLU_CULLING);
            m_scene.LoadScene();
            m_scene.Initialize();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.MatrixMode(OpenGL.GL_PROJECTION);      
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width / height, 1.0f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();            
        }


        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // drawing objects which can be manipuleted by keyboard events
            gl.PushMatrix();
            {

                // setting viewport to take full screen
                gl.Viewport(0, 0, m_width, m_height);

                // transformations based on keyboard events will apply to all of the objects on the scene
                gl.Translate(0.0f, 15.0f, -m_sceneDistance);
                gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
                gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

                // person
                gl.PushMatrix();
                DrawMeshModel(gl);
                gl.PopMatrix();

                // podloga
                gl.PushMatrix();
                DrawPodloga(gl);
                gl.PopMatrix();

                //stepenice
                gl.PushMatrix();
                DrawCube(gl);
                gl.PopMatrix();


            }
            gl.PopMatrix();

            // right bottom corner text
            gl.PushMatrix();
            {
                // placing text by redefining viewport
                gl.Viewport((int)(m_width / 1.1), 0, m_width / 2, m_height / 2);
                DrawTextRightBottomCorner(gl);
            }
            gl.PopMatrix();

            // reset the viewport
            gl.Viewport(0, 0, m_width, m_height);

            gl.Flush();
        }




        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

      

            private void DrawMeshModel(OpenGL gl)
        {

            gl.PushMatrix();
            gl.Translate(-180, -205, -100f);
            gl.Scale(2.5f, 2.5f, 2.5f);  
            gl.Rotate(0.0f, 135f, 0);

            m_scene.Draw();

            gl.PopMatrix();
            gl.Flush();
        }

        private void DrawCube(OpenGL gl)
        {
            //prva stepenica
            gl.PushMatrix();
            gl.Rotate(0.0f, -35f, -0.0f);
            gl.Translate(-250, -274f, -40f);
            gl.Scale(150f, 20f, 30f);
         
       
            gl.Color(0.6f, 0.1f, 0.3f);
            Cube stepenica = new Cube();
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            //druga stepenica
            gl.Translate(0, 1f, -2f);
            gl.Scale(1f, 2f, 1f);
            gl.Color(1f, 0.1f, 0.3f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);


            //treca stepenica
            gl.Translate(0, 0.5f, -2f);
            gl.Scale(1f, 1.5f, 1f);
            gl.Color(0.6f, 1f, 0.3f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            //cetvrta stepenica
            gl.Translate(0, 0.33f, -2f);
            gl.Scale(1f, 1.33f, 1f);
            gl.Color(0.6f, 0.1f, 1f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            
            //peta stepenica
            gl.Translate(0, 0.25f, -2f);
            gl.Scale(1f, 1.25f, 1f);
            gl.Color(1f, 1f, 0.5f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            //sesta stepenica
            gl.Translate(0, 0.17f, -2f);
            gl.Scale(1f, 1.17f, 1f);
            gl.Color(1f, 0.5f, 1f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            //sedma stepenica
            gl.Translate(0, 0.14f, -2f);
            gl.Scale(1f, 1.14f, 1f);
            gl.Color(0.5f, 1f, 1f);
            stepenica.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);

            gl.PopMatrix();
        }

    
        private void DrawPodloga(OpenGL gl)
        {
            float[] podloga =
            {
                -1000.0f, -300.0f,  170.0f,
                -1000.0f, -300.0f, -1000.0f,
                 1000.0f, -300.0f, -1000.0f,
                 1000.0f, -300.0f,  170.0f,
            };

            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Begin(OpenGL.GL_QUADS);
            int i = 0;
            while (i < podloga.Length)
            {
                gl.Vertex(podloga[i], podloga[i + 1], podloga[i + 2]);

                i += 3;
            }

            gl.End();

        }

        private void DrawTextRightBottomCorner(OpenGL gl)
        {
        
            gl.Viewport((int)(m_width / 1.2), 0, m_width / 2, m_height / 2);
            gl.DrawText(160, 125, 1.0f, 0.0f, 0.0f, "Verdana Italic", 10, "Predmet: Racunarska grafika");
            gl.DrawText(160, 100, 1.0f, 0.0f, 0.0f, "Verdana Italic", 10, "Sk.god: 2019/20.");
            gl.DrawText(160, 75, 1.0f, 0.0f, 0.0f, "Verdana Italic", 10, "Ime: Milica");
            gl.DrawText(160, 50, 1.0f, 0.0f, 0.0f, "Verdana Italic", 10, "Prezime: Culibrk");
            gl.DrawText(160, 25, 1.0f, 0.0f, 0.0f, "Verdana Italic", 10, "Sifra zad: 11.1");

        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}

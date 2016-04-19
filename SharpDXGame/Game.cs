// Copyright (c) 2016 Egor Anisimov

namespace SharpDXGame
{
    using System;
    using System.Drawing;
    using SharpDX.Windows;

    public class Game : IDisposable
    {
        /// <summary>
        /// Input service instance.
        /// </summary>
        private InputService inputService;

        /// <summary>
        /// Graphics service instance.
        /// </summary>
        private GraphicsService graphicsService;

        private const int Width = 1280;
        private const int Height = 720;

        private RenderForm renderForm;

        public Game()
        {
            this.renderForm = new RenderForm("MS project");
            this.renderForm.ClientSize = new Size(Width, Height);
            this.renderForm.AllowUserResizing = false;

            this.inputService = new InputService();
            this.graphicsService = new GraphicsService(this.renderForm);
        }

        public void Run()
        {
            RenderLoop.Run(renderForm, this.RenderCallback);
        }

        public void Dispose()
        {
            this.inputService.Dispose();
            this.graphicsService.Dispose();
            this.renderForm.Dispose();
        }

        private void RenderCallback()
        {
            this.graphicsService.Frame();
        }
    }
}

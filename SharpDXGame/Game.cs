using System;
using System.Drawing;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Windows;
using D3D11 = SharpDX.Direct3D11;

namespace SharpDXGame
{
    public class Game : IDisposable
    {
        private const int Width = 1280;
        private const int Height = 720;

        private RenderForm renderForm;

        private D3D11.Device d3dDevice;
        private D3D11.DeviceContext d3dDeviceContext;
        private SwapChain swapChain;
        private D3D11.RenderTargetView renderTargetView;

        private D3D11.Buffer triangleVertexBuffer;

        private D3D11.VertexShader vertexShader;
        private D3D11.PixelShader pixelShader;

        private D3D11.InputElement[] inputElements = new D3D11.InputElement[]
        {
            new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0)
        };
        private ShaderSignature inputSignature;
        private D3D11.InputLayout inputLayout;

        private Viewport viewport;

        public Game()
        {
            this.renderForm = new RenderForm("MS project");
            this.renderForm.ClientSize = new Size(Width, Height);
            this.renderForm.AllowUserResizing = false;

            this.InitializeDeviceResources();
            this.InitializeTriangle();
            this.InitializeShaders();
        }

        private void InitializeTriangle()
        {
            var vertices = new Vector3[] 
            {
                new Vector3(0f, 0f, 0f),
                new Vector3(1f, 0f, 0f),
                new Vector3(0f, 1f, 0f)
            };

            this.triangleVertexBuffer = D3D11.Buffer.Create<Vector3>(
                this.d3dDevice,
                D3D11.BindFlags.VertexBuffer,
                vertices);

        }

        public void Run()
        {
            RenderLoop.Run(renderForm, this.RenderCallback);
        }

        public void Dispose()
        {
            this.renderForm.Dispose();
            this.swapChain.Dispose();
            this.renderTargetView.Dispose();
            this.d3dDevice.Dispose();
            this.d3dDeviceContext.Dispose();
            this.triangleVertexBuffer.Dispose();
            this.pixelShader.Dispose();
            this.vertexShader.Dispose();
            this.inputLayout.Dispose();
            this.inputSignature.Dispose();
        }

        private void RenderCallback()
        {
            this.Draw();
        }

        private void Draw()
        {
            this.d3dDeviceContext.ClearRenderTargetView(
                this.renderTargetView, new SharpDX.Color(32, 103, 178));

            d3dDeviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(triangleVertexBuffer, Utilities.SizeOf<Vector3>(), 0));
            d3dDeviceContext.Draw(3, 0);

            swapChain.Present(1, PresentFlags.None);
        }

        private void InitializeDeviceResources()
        {
            var backBufferDesc = new ModeDescription(
                Width,
                Height,
                new Rational(60, 1),
                Format.B8G8R8A8_UNorm);

            var swapChainDesc = new SwapChainDescription()
            {
                ModeDescription = backBufferDesc,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                OutputHandle = renderForm.Handle,
                IsWindowed = true
            };

            D3D11.Device.CreateWithSwapChain(
                DriverType.Hardware,
                D3D11.DeviceCreationFlags.None,
                swapChainDesc,
                out d3dDevice,
                out swapChain);
            d3dDeviceContext = d3dDevice.ImmediateContext;

            using (D3D11.Texture2D backBuffer = swapChain.GetBackBuffer<D3D11.Texture2D>(0))
            {
                this.renderTargetView = new D3D11.RenderTargetView(d3dDevice, backBuffer);
            }

            d3dDeviceContext.OutputMerger.SetRenderTargets(this.renderTargetView);

            this.viewport = new Viewport(0, 0, Width, Height);
            this.d3dDeviceContext.Rasterizer.SetViewport(this.viewport);
        }

        private void InitializeShaders()
        {
            using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile("vertexShader.hlsl", "main", "vs_4_0", ShaderFlags.Debug))
            {
                this.inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
                this.vertexShader = new D3D11.VertexShader(this.d3dDevice, vertexShaderByteCode);
            }

            using (var pixelShaderCode = ShaderBytecode.CompileFromFile
                ("PixelShader.hlsl", "main", "ps_4_0", ShaderFlags.Debug))
            {
                this.pixelShader = new D3D11.PixelShader(
                    this.d3dDevice, pixelShaderCode);
            }

            this.d3dDeviceContext.VertexShader.Set(this.vertexShader);
            this.d3dDeviceContext.PixelShader.Set(this.pixelShader);

            this.d3dDeviceContext.InputAssembler.PrimitiveTopology
                = PrimitiveTopology.TriangleList;

            this.inputLayout = new D3D11.InputLayout(this.d3dDevice, this.inputSignature, this.inputElements);
            d3dDeviceContext.InputAssembler.InputLayout = this.inputLayout;
        }
    }
}

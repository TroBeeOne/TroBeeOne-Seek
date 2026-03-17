# Seek - Visual AI (Local-First PWA)

Seek is a lightweight, privacy-focused Progressive Web App (PWA) designed for mobile-first visual analysis. It uses a .NET 8 backend to bridge a Vue 3 frontend with a local **Ollama** instance running Vision-Language (VL) models.

![Seek UI](Seek/SeekProject/wwwroot/t1-icon-192.png)

## 🚀 Features

- **Mobile-First PWA**: Installable on iOS/Android for a native app experience.
- **Local AI Power**: Leverages Ollama for analysis—your images never leave your infrastructure.
- **Vision Workflows**:
  - **Capture & Confirm**: Take a photo, review it, and add a custom prompt before sending.
  - **Quick Actions**: One-tap buttons for Barcodes (with clickable links), Inventory counting, and OCR.
  - **Image Downloads**: Save your captured photos directly to your device.
- **Streaming Responses**: Real-time AI output for an interactive feel.
- **Secure by Default**: Designed to run behind Cloudflare Tunnels for secure remote access without port forwarding.

## 🛠️ Tech Stack

- **Frontend**: Vue 3 (ESM), Vanilla CSS, Marked.js (Markdown rendering).
- **Backend**: .NET 8 Web API.
- **AI Engine**: [Ollama](https://ollama.com/) (running `qwen2-vl` or `qwen3-vl`).
- **Deployment**: IIS / Windows, Cloudflare Tunnel.

## 📋 Prerequisites

1.  **Ollama**: Install [Ollama](https://ollama.com/) on your workstation.
2.  **Vision Model**: Pull a vision-capable model:
    ```bash
    ollama pull qwen3-vl:8b
    ```
3.  **.NET 8 SDK**: For building and running the backend.

## ⚙️ Setup & Configuration

### 1. Configure Ollama
In `Seek/SeekProject/appsettings.json`, ensure the URL and model match your setup:
```json
"Ollama": {
  "Url": "http://localhost:11434",
  "Model": "qwen3-vl:8b"
}
```

### 2. Local Development
Run the project from the root directory:
```bash
dotnet run --project Seek/SeekProject/SeekProject.csproj
```
Open `http://localhost:5000` in your browser.

### 3. Production / IIS Deployment
- **Publish**: Use Visual Studio to publish the project to a folder.
- **IIS Setup**:
  - Point your IIS Site to the published `wwwroot` folder.
  - Ensure the **ASP.NET Core Runtime 8.0** Hosting Bundle is installed.
- **Cloudflare Tunnel**:
  - Route your tunnel to the local IIS port (e.g., `http://localhost:80`).
  - Access the site on your phone via your custom domain.

## 📱 PWA Installation
1. Open the app in your mobile browser (Safari on iOS, Chrome on Android).
2. Tap the **Share/Menu** button.
3. Select **"Add to Home Screen"**.
4. The app will now appear on your home screen and run in standalone mode without browser bars.

## 📄 License
MIT

# YouTube & Facebook Video Downloader API

A modern, developer-friendly .NET 9 Web API for retrieving video information and download options from YouTube **and Facebook** using [yt-dlp](https://github.com/yt-dlp/yt-dlp).

---

## 🛠️ Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [yt-dlp](https://github.com/yt-dlp/yt-dlp) (must be installed and available in your system PATH)

---

## 🚀 Features

- 🔍 Fetch video metadata (title, description, duration, etc.)
- 🎞️ Get available video formats and quality options
- 🎵 Support for muxed (video + audio) streams
- 🖼️ Thumbnail information
- 🛡️ Robust error handling and development mode stack traces
- 🌐 Supports both **YouTube** and **Facebook** video URLs

---

## ⚡ Quick Start

1. **Clone the repository:**
2. **Restore dependencies:**
3. **Install yt-dlp:**
- **Windows:**  
  Download from [yt-dlp releases](https://github.com/yt-dlp/yt-dlp/releases) or use [Chocolatey](https://chocolatey.org/):choco install yt-dlp- 
- **Linux/macOS:** sudo curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /usr/local/bin/yt-dlp
sudo chmod a+rx /usr/local/bin/yt-dlp
4. **Run the application:**

---

## 📚 API Usage

### `GET /video/info`

Retrieve video information and available formats for YouTube or Facebook videos.

**Query Parameters:**
- `videoUrl` (required): The URL of the video to analyze


---

## 📝 Notes

- Make sure `yt-dlp` is up-to-date for best compatibility with YouTube and Facebook.
- This API is for educational and personal use. Respect the terms of service of the platforms you access.

---

## 👨‍💻 Author

Made with ❤️ by **M-Nimesh**  
[GitHub](https://github.com/M-Nimesh)

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).
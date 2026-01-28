# Multi AI Chat

**.NET 8 Web API** ve **React (TypeScript)** ile geliştirilmiş, **Docker** üzerinde çalışan, genişletilebilir mimariye sahip (Factory Pattern) çok modelli bir yapay zeka sohbet uygulamasıdır.

## Tech Stack

- **Backend:** .NET 8, EF Core, SQL Server
- **Frontend:** React 18, TypeScript, Vite, Nginx
- **DevOps:** Docker, Docker Compose

## 🔑 Önemli: Kurulum ve .env Dosyası

Projeyi sorunsuz test edebilmeniz ve API anahtarı (Gemini/OpenAI Key) alma süreçleriyle uğraşmamanız için **gerekli `.env` dosyasını doğrudan benden talep edebilirsiniz.**

> ℹ️ **Bilgi:** Hazır yapılandırılmış `.env` dosyasını size ilettiğimde, dosyayı projenin ana dizinine (root folder) yapıştırmanız yeterli olacaktır.

## 🐳 Kurulum ve Çalıştırma

**1. Repoyu Klonlayın:**
```bash
git clone https://github.com/yusufutkurak/MultiChatApp
cd MultiChatApp
```

**2. .env Dosyasını Ekleyin:**
Benden temin ettiğiniz `.env` dosyasını projenin ana dizinine ekleyin.

**3. Projeyi Başlatın:**
Terminalde şu komutu çalıştırın:
```bash
docker-compose up --build
```

## 🔗 Erişim Linkleri

Uygulama ayağa kalktığında aşağıdaki adreslerden erişebilirsiniz:

- **Uygulama Arayüzü:** [http://localhost:3000](http://localhost:3000)
- **API Swagger Dokümantasyonu:** [http://localhost:7050/swagger](http://localhost:7050/swagger)

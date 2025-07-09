# VoiceBot Modular Web Application

## Overview
VoiceBot is a modular, scalable web application built with .NET 8 (backend), MySQL (database), Vue.js (frontend), and Python FastAPI microservices for CPU/GPU processing. It supports TTS, STT, LLM-based conversational AI, CSV content management, and real-time voice communication over WebSockets, with seamless provider and hardware switching.

## Structure
- `backend/` - .NET 8 WebAPI for TTS, STT, LLM, admin, and WebSocket endpoints
- `frontend/` - Vue.js 3 (Vite) frontend for user/admin interfaces
- `python-services/` - FastAPI microservice for TTS/STT/LLM with CPU/GPU switching
- `db/` - MySQL schema and migrations
- `docker/` - Dockerfiles and docker-compose for deployment

## Features
- Modular TTS/STT/LLM with provider/hardware switching
- Real-time voice chat via WebSockets
- Admin dashboard for CSV and provider management
- Secure, extensible, and scalable architecture

## Ports
- **MySQL**: `3306`
- **Backend (.NET 8 WebAPI)**: `5000` (Docker, mapped to container port 80)
- **Frontend (Vue.js/Nginx)**: `8080` (Docker, mapped to container port 80)
- **Python FastAPI Service**: `8000`

## Build & Run Scripts

### Development (local, non-docker)
1. **MySQL**: Ensure MySQL is running and matches `db/schema.sql`.
2. **Backend**
   ```sh
   dotnet build backend
   dotnet run --project backend
   ```
3. **Frontend**
   ```sh
   cd frontend
   npm install
   npm run dev
   ```
   - Access at http://localhost:5173 (default Vite dev port)
4. **Python Service**
   ```sh
   cd python-services
   pip install -r requirements.txt
   uvicorn app.main:app --host 0.0.0.0 --port 8000
   ```

### Production (Docker Compose)
1. Build and run all services:
   ```sh
   docker compose -f docker/docker-compose.yml up --build
   ```
   - Access frontend at http://localhost:8080
   - Backend API at http://localhost:5000
   - Python service at http://localhost:8000
   - MySQL at localhost:3306

## Documentation
- See `.github/copilot-instructions.md` for workspace-specific coding guidelines
- Each module/service is documented in-code

## License
Open-source, extensible for research and production use.

from fastapi import FastAPI, UploadFile, File, HTTPException
from fastapi.responses import JSONResponse
from typing import Optional, Dict, Callable
import torch
from datetime import datetime

app = FastAPI()

# --- Provider Base Classes ---
class TTSProvider:
    def synthesize(self, text, voice, language, emotion, hardware):
        raise NotImplementedError

class STTProvider:
    def transcribe(self, file, model, language, hardware):
        raise NotImplementedError

class LLMProvider:
    def chat(self, prompt, hardware, session_id):
        raise NotImplementedError

# --- Example Providers ---
class DummyTTSProvider(TTSProvider):
    def synthesize(self, text, voice, language, emotion, hardware):
        # Simulate audio bytes
        return b"AUDIO_BYTES"

class DummySTTProvider(STTProvider):
    def transcribe(self, file, model, language, hardware):
        # Simulate text
        return "Simulated transcription."

class DummyLLMProvider(LLMProvider):
    def chat(self, prompt, hardware, session_id):
        return "Simulated LLM response."

# --- Provider Registry ---
tts_providers: Dict[str, Callable[[], TTSProvider]] = {
    "Dummy": DummyTTSProvider,
    # Add real providers here
}
stt_providers: Dict[str, Callable[[], STTProvider]] = {
    "Dummy": DummySTTProvider,
    # Add real providers here
}
llm_providers: Dict[str, Callable[[], LLMProvider]] = {
    "Dummy": DummyLLMProvider,
    # Add real providers here
}

def get_tts_provider(name: str) -> TTSProvider:
    if name in tts_providers:
        return tts_providers[name]()
    raise HTTPException(status_code=400, detail=f"TTS provider '{name}' not found.")

def get_stt_provider(name: str) -> STTProvider:
    if name in stt_providers:
        return stt_providers[name]()
    raise HTTPException(status_code=400, detail=f"STT provider '{name}' not found.")

def get_llm_provider(name: str) -> LLMProvider:
    if name in llm_providers:
        return llm_providers[name]()
    raise HTTPException(status_code=400, detail=f"LLM provider '{name}' not found.")

@app.get("/health")
def health():
    return {"status": "ok", "gpu": torch.cuda.is_available()}

# In-memory log and metrics storage for demonstration
logs = [
    {"id": 1, "timestamp": datetime.utcnow().isoformat(), "message": "System started."},
    {"id": 2, "timestamp": datetime.utcnow().isoformat(), "message": "Dummy provider loaded."}
]
metrics = {
    "requests_tts": 0,
    "requests_stt": 0,
    "requests_llm": 0,
    "active_sessions": 1
}

@app.get("/logs")
def get_logs():
    return JSONResponse(content=logs)

@app.get("/metrics")
def get_metrics():
    return JSONResponse(content=metrics)

@app.post("/tts")
def tts(text: str, provider: str = 'Dummy', voice: str = 'default', language: str = 'en', emotion: Optional[str] = None, hardware: str = 'cpu'):
    metrics["requests_tts"] += 1
    tts_provider = get_tts_provider(provider)
    # Hardware switching logic (example: check torch.cuda.is_available())
    if hardware == 'gpu' and not torch.cuda.is_available():
        raise HTTPException(status_code=400, detail="GPU not available.")
    audio_bytes = tts_provider.synthesize(text, voice, language, emotion, hardware)
    logs.append({"id": len(logs)+1, "timestamp": datetime.utcnow().isoformat(), "message": f"TTS request ({provider}, {hardware})"})
    return {"audio": audio_bytes.hex()}

@app.post("/stt")
def stt(file: UploadFile = File(...), provider: str = 'Dummy', model: str = 'large-v3', language: str = 'en', hardware: str = 'cpu'):
    metrics["requests_stt"] += 1
    stt_provider = get_stt_provider(provider)
    if hardware == 'gpu' and not torch.cuda.is_available():
        raise HTTPException(status_code=400, detail="GPU not available.")
    # Read file bytes
    audio_bytes = file.file.read()
    text = stt_provider.transcribe(audio_bytes, model, language, hardware)
    logs.append({"id": len(logs)+1, "timestamp": datetime.utcnow().isoformat(), "message": f"STT request ({provider}, {hardware})"})
    return {"text": text}

@app.post("/llm")
def llm(prompt: str, provider: str = 'Dummy', hardware: str = 'cpu', session_id: Optional[str] = None):
    metrics["requests_llm"] += 1
    llm_provider = get_llm_provider(provider)
    if hardware == 'gpu' and not torch.cuda.is_available():
        raise HTTPException(status_code=400, detail="GPU not available.")
    response = llm_provider.chat(prompt, hardware, session_id)
    logs.append({"id": len(logs)+1, "timestamp": datetime.utcnow().isoformat(), "message": f"LLM request ({provider}, {hardware})"})
    return {"response": response}

"""
Extensibility:
- Add new providers by subclassing TTSProvider, STTProvider, or LLMProvider and registering them in the provider dicts.
- Hardware switching is handled by the 'hardware' parameter and torch.cuda.is_available().
- This pattern supports easy addition of new providers and hardware types.
"""

<template>
  <div>
    <h2>Voice Chat</h2>
    <div>Status: <span :class="statusClass">{{ connectionStatus }}</span></div>
    <button @click="startRecording" :disabled="isRecording">Start</button>
    <button @click="stopRecording" :disabled="!isRecording">Stop</button>
    <audio ref="audioPlayer" controls></audio>
    <div v-if="error" class="error">{{ error }}</div>
  </div>
</template>
<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import * as signalR from '@microsoft/signalr';

const audioPlayer = ref(null);
const connectionStatus = ref('Disconnected');
const statusClass = ref('disconnected');
const isRecording = ref(false);
const error = ref('');
let connection = null;
let mediaRecorder = null;
let audioChunks = [];

// Helper to get backend base URL
function getBackendUrl() {
  // Use VITE_BACKEND_URL if set, otherwise default to same origin
  return import.meta.env.VITE_BACKEND_URL || window.location.origin;
}

function connect() {
  connection = new signalR.HubConnectionBuilder()
    .withUrl(getBackendUrl() + '/voicehub')
    .withAutomaticReconnect()
    .build();

  connection.on('ReceiveAudio', (audioData) => {
    const blob = new Blob([new Uint8Array(audioData)], { type: 'audio/webm' });
    audioPlayer.value.src = URL.createObjectURL(blob);
    audioPlayer.value.play();
  });

  connection.onreconnecting(() => {
    connectionStatus.value = 'Reconnecting...';
    statusClass.value = 'reconnecting';
  });
  connection.onreconnected(() => {
    connectionStatus.value = 'Connected';
    statusClass.value = 'connected';
  });
  connection.onclose(() => {
    connectionStatus.value = 'Disconnected';
    statusClass.value = 'disconnected';
  });

  connection.start()
    .then(() => {
      connectionStatus.value = 'Connected';
      statusClass.value = 'connected';
    })
    .catch(err => {
      error.value = 'Connection failed: ' + err;
      connectionStatus.value = 'Disconnected';
      statusClass.value = 'disconnected';
    });
}

function startRecording() {
  error.value = '';
  if (!navigator.mediaDevices) {
    error.value = 'Media devices not supported.';
    return;
  }
  navigator.mediaDevices.getUserMedia({ audio: true })
    .then(stream => {
      mediaRecorder = new MediaRecorder(stream, { mimeType: 'audio/webm' });
      audioChunks = [];
      mediaRecorder.ondataavailable = e => {
        if (e.data.size > 0) audioChunks.push(e.data);
      };
      mediaRecorder.onstop = () => {
        const audioBlob = new Blob(audioChunks, { type: 'audio/webm' });
        audioBlob.arrayBuffer().then(buffer => {
          if (connection && connection.state === 'Connected') {
            connection.invoke('SendAudio', Array.from(new Uint8Array(buffer)));
          }
        });
      };
      mediaRecorder.start();
      isRecording.value = true;
    })
    .catch(err => {
      error.value = 'Could not start recording: ' + err;
    });
}

function stopRecording() {
  if (mediaRecorder && isRecording.value) {
    mediaRecorder.stop();
    isRecording.value = false;
  }
}

onMounted(connect);
onUnmounted(() => {
  if (connection) connection.stop();
});
</script>
<style scoped>
.connected { color: green; }
.reconnecting { color: orange; }
.disconnected { color: red; }
.error { color: red; margin-top: 1em; }
</style>

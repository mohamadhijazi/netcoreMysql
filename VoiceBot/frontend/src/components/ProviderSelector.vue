<template>
  <div>
    <label>Provider:</label>
    <select v-model="selectedProvider">
      <option v-for="provider in providers" :key="provider" :value="provider">{{ provider }}</option>
    </select>
    <label>Hardware:</label>
    <select v-model="selectedHardware">
      <option value="cpu">CPU</option>
      <option value="gpu">GPU</option>
    </select>
  </div>
</template>
<script setup>
import { ref } from 'vue';
const providers = ['Whisper', 'ElevenLabs', 'XTTS', 'Ollama'];
const selectedProvider = ref(providers[0]);
const selectedHardware = ref('cpu');
</script>

<template>
  <div>
    <h2>Settings</h2>
    <p>Select your preferred provider and hardware for TTS, STT, and LLM modules.</p>
    <ProviderSelector />
  </div>
</template>
<script setup>
import ProviderSelector from './ProviderSelector.vue';
</script>
<style scoped>
/* Add settings-specific styles here */
</style>

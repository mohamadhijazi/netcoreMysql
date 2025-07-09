<template>
  <div>
    <h2>Dashboard</h2>
    <p>Welcome to the VoiceBot dashboard. Here you can monitor system health, recent activity, and usage metrics.</p>
    <div class="metrics">
      <div class="metric">
        <span class="label">System Health:</span>
        <span class="value" :class="{ healthy: health.status === 'ok', unhealthy: health.status !== 'ok' }">{{ health.status }}</span>
        <span v-if="health.gpu">(GPU available)</span>
        <span v-else>(CPU only)</span>
      </div>
      <!-- Add more metrics as needed -->
    </div>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue';
const health = ref({ status: 'loading', gpu: false });
onMounted(async () => {
  try {
    const res = await fetch('/python/health');
    health.value = await res.json();
  } catch {
    health.value = { status: 'unreachable', gpu: false };
  }
});
</script>
<style scoped>
.metrics { margin-top: 2em; }
.metric { margin-bottom: 1em; }
.label { font-weight: bold; }
.value.healthy { color: green; }
.value.unhealthy { color: red; }
</style>

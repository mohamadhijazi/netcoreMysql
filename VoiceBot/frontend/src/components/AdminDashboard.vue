<template>
  <div>
    <h2>Admin Dashboard</h2>
    <div class="admin-actions">
      <input type="file" ref="csvInput" accept=".csv" style="display:none" @change="onCsvSelected" />
      <button @click="triggerCsvUpload">Upload CSV</button>
      <button @click="exportCsv">Export CSV</button>
    </div>
    <div v-if="logs.length">
      <h3>Logs</h3>
      <ul>
        <li v-for="log in logs" :key="log.id">{{ log.timestamp }} - {{ log.message }}</li>
      </ul>
    </div>
    <div v-if="metrics">
      <h3>Metrics</h3>
      <ul>
        <li v-for="(value, key) in metrics" :key="key">{{ key }}: {{ value }}</li>
      </ul>
    </div>
    <div v-if="error" class="error">{{ error }}</div>
  </div>
</template>
<script setup>
import { ref, onMounted } from 'vue';
const logs = ref([]);
const metrics = ref(null);
const error = ref('');
const csvInput = ref(null);

function triggerCsvUpload() {
  csvInput.value.click();
}

async function onCsvSelected(e) {
  error.value = '';
  const file = e.target.files[0];
  if (!file) return;
  const formData = new FormData();
  formData.append('file', file);
  try {
    await fetch('/api/csv/import', { method: 'POST', body: formData });
    fetchLogs();
  } catch (err) {
    error.value = 'CSV upload failed.';
  }
}

async function exportCsv() {
  error.value = '';
  try {
    const res = await fetch('/api/csv/export');
    const blob = await res.blob();
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'export.csv';
    a.click();
    window.URL.revokeObjectURL(url);
  } catch (err) {
    error.value = 'CSV export failed.';
  }
}

async function fetchLogs() {
  try {
    const res = await fetch('/api/logs');
    logs.value = await res.json();
  } catch {
    logs.value = [];
  }
}

async function fetchMetrics() {
  try {
    const res = await fetch('/api/metrics');
    metrics.value = await res.json();
  } catch {
    metrics.value = null;
  }
}

onMounted(() => {
  fetchLogs();
  fetchMetrics();
});
</script>
<style scoped>
.admin-actions { margin-bottom: 1em; }
.error { color: red; margin-top: 1em; }
</style>

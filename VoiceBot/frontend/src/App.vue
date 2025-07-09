<script setup>
// No setup needed for main layout
</script>

<template>
  <div id="app">
    <nav class="navbar">
      <router-link to="/" class="nav-link">Dashboard</router-link>
      <router-link to="/chat" class="nav-link">Voice Chat</router-link>
      <router-link to="/admin" class="nav-link">Admin</router-link>
      <router-link to="/settings" class="nav-link">Settings</router-link>
    </nav>
    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
#app {
  font-family: 'Segoe UI', Arial, sans-serif;
  background: #f8f9fa;
  min-height: 100vh;
}
.navbar {
  display: flex;
  gap: 2rem;
  background: #222;
  padding: 1rem 2rem;
}
.nav-link {
  color: #fff;
  text-decoration: none;
  font-weight: 500;
  font-size: 1.1rem;
}
.nav-link.router-link-exact-active {
  color: #42b883;
  border-bottom: 2px solid #42b883;
}
.main-content {
  padding: 2rem;
}
</style>

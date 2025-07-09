import { createRouter, createWebHistory } from 'vue-router';
import Dashboard from '../components/Dashboard.vue';
import VoiceChat from '../components/VoiceChat.vue';
import AdminDashboard from '../components/AdminDashboard.vue';
import Settings from '../components/Settings.vue';

const routes = [
  { path: '/', name: 'Dashboard', component: Dashboard },
  { path: '/chat', name: 'VoiceChat', component: VoiceChat },
  { path: '/admin', name: 'Admin', component: AdminDashboard },
  { path: '/settings', name: 'Settings', component: Settings },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;

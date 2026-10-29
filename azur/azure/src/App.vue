<script setup lang="ts">
import { RouterView } from 'vue-router'
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'

const authStore = useAuthStore()

onMounted(() => {
  authStore.loadFromStorage()
})
</script>

<template>
  <RouterView />
</template>

<style>
* {
  box-sizing: border-box;
  margin: 0;
  padding: 0;
}

body {
  font-family: 'Segoe UI', Tahoma, sans-serif;
  background: #f0f0f0;
  color: #333;
}

#app {
  min-height: 100vh;
}
</style>
<script setup lang="ts">
import { RouterView, useRoute } from 'vue-router' // Добавлен useRoute
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import FooterComponent from '@/components/Footer.vue'
import HeaderComponent from '@/components/Header.vue'

const authStore = useAuthStore()
const route = useRoute() // Инициализируем получение текущего маршрута

onMounted(() => {
  authStore.loadFromStorage()
})
</script>

<template>
  <!-- Компоненты показываются только если флаги скрытия НЕ установлены в true -->
  <HeaderComponent v-if="!route.meta.hideHeader"/>
  
  <RouterView />
  
  <FooterComponent v-if="!route.meta.hideFooter"/>
</template>

<style>
/* Исправлена опечатка в CSS сбросе: вместо "-" должно быть "*" */
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
  display: flex; /* Добавлено, чтобы прижимать футер к низу */
  flex-direction: column;
}

/* Растягиваем RouterView на всю свободную высоту, чтобы футер был в самом низу */
#app > :nth-child(2) {
  flex: 1;
}
</style>
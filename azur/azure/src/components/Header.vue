<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'

const router = useRouter()
const route = useRoute()
const auth = useAuthStore()
const cartStore = useCartStore()

const isActive = (path: string) => route.path === path

// 1. Берем готовое число из нашего нового стора
const cartItemsCount = computed(() => cartStore.totalCount)

// 2. Заставляем шапку скачать данные при входе на сайт
onMounted(() => {
  if (auth.isAuthenticated) {
    cartStore.loadCartFromServer()
  }
})
</script>

<template>
  <header class="main-header">
    <div class="brand-section" @click="router.push('/catalog')">
      <img src="/logo.png" alt="Azure Logo" class="header-logo" />
      <span class="brand-name">Azure</span>
    </div>

    <nav class="header-nav">
      <button 
        class="nav-btn" 
        :class="{ active: isActive('/catalog') }" 
        @click="router.push('/catalog')"
      >
        Главная
      </button>
      
      <button 
        class="nav-btn" 
        :class="{ active: isActive('/cart') }" 
        @click="router.push('/cart')"
      >
        <!-- 3. Выводим это число -->
        Корзина <span v-if="cartItemsCount > 0">({{ cartItemsCount }})</span>
      </button>
      
      <button 
        class="nav-btn" 
        :class="{ active: isActive('/profile') }"
        @click="router.push(auth.isAuthenticated ? '/profile' : '/login')"
      >
        {{ auth.isAuthenticated ? 'Личный кабинет' : 'Войти' }}
      </button>
    </nav>
  </header>
</template>

<style scoped>
.main-header {
  position: relative; 
  background-color: #C8D3DF;
  padding: 12px 24px;
  display: flex;
  align-items: center;
  justify-content: space-between; 
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  /* border-bottom-left-radius: 20px;
  border-bottom-right-radius: 20px;
  margin-bottom: 20px; */
}

.brand-section {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
}

.header-logo {
  width: 45px;
  height: 45px;
  object-fit: contain;
  border-radius: 10px;
  background: white;
  padding: 4px;
  box-shadow: 0 2px 6px rgba(0,0,0,0.1);
}

.brand-name {
  font-size: 22px;
  font-weight: 700;
  color: #1a3a6b;
}

.header-nav {
  position: absolute; 
  left: 50%; 
  transform: translateX(-50%); 
  display: flex;
  align-items: center;
  gap: 16px;
}

.nav-btn {
  padding: 8px 24px;
  border-radius: 18px;
  border: none;
  background-color: #6F8AA6;
  color: white;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1);
}

.nav-btn:hover {
  background-color: #557290;
  transform: translateY(-1px);
}

.nav-btn.active {
  background-color: #4A6785;
}

@media (max-width: 768px) {
  .main-header {
    flex-direction: column;
    gap: 16px;
  }
  .header-nav {
    position: static; 
    transform: none;
  }
}
</style>
<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { productsApi } from '@/api/products'
import { useAuthStore } from '@/stores/auth'
import type { Product } from '@/types/product'
import ProductCard from '@/components/ProductCard.vue'

const router = useRouter()
const auth = useAuthStore()

const allProducts = ref<Product[]>([])
const search = ref('')
const category = ref('Все категории')

onMounted(async () => {
  try {
    const products = await productsApi.getAll()
    // Фильтруем по статусу 'Активен' (согласно полю enum('Активен','Удален') в базе MySQL)
    allProducts.value = products.filter((p: any) => p.status !== 'Удален' && p.Status !== 'Удален')
  } catch (e) {
    console.error('Ошибка загрузки товаров:', e)
  }
})

// Универсальная функция для получения названия категории независимо от регистра/структуры C# DTO
const getCategoryName = (p: any) => p.category || p.Category || p.categoryTitle || p.CategoryTitle || 'Прочее'

const categories = computed(() => {
  const cats = [...new Set(allProducts.value.map(p => getCategoryName(p)))]
  return ['Все категории', ...cats]
})

const filtered = computed(() =>
  allProducts.value.filter((p: any) => {
    const title = p.title || p.Title || ''
    const catName = getCategoryName(p)
    const matchesSearch = title.toLowerCase().includes(search.value.toLowerCase())
    const matchesCategory = category.value === 'Все категории' || catName === category.value
    return matchesSearch && matchesCategory
  })
)
</script>

<template>
  <div class="catalog-wrapper">
    <!-- Шапка приложения строго по макету -->
    <header class="catalog-header">
      <div class="header-top">
        <!-- Поле поиска с лупой -->
        <div class="search-wrapper">
          <input v-model="search" type="text" placeholder="Поиск товаров..." class="search-input" />
          <svg class="search-icon" viewBox="0 0 24 24" fill="none" stroke="#666" stroke-width="2">
            <circle cx="11" cy="11" r="8"></circle>
            <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
          </svg>
        </div>

        <!-- Выпадающий список категорий -->
        <select v-model="category" class="category-select">
          <option v-for="c in categories" :key="c" :value="c">{{ c }}</option>
        </select>
      </div>

      <!-- Кнопки навигации -->
      <div class="header-nav">
        <button class="nav-btn active" @click="router.push('/catalog')">Главная</button>
        <button class="nav-btn" @click="router.push('/cart')">Корзина</button>
        <button class="nav-btn" @click="router.push(auth.isAuthenticated ? '/profile' : '/login')">
          {{ auth.isAuthenticated ? 'Личный кабинет' : 'Войти' }}
        </button>
      </div>
    </header>

    <!-- Основная сетка товаров -->
    <main class="catalog-content">
      <div v-if="filtered.length === 0" class="empty-msg">
        Товары не найдены
      </div>
      <div v-else class="grid">
        <ProductCard v-for="p in filtered" :key="p.idProduct" :product="p" />
      </div>
    </main>
  </div>
</template>

<style scoped>
/* Полноэкранный фоновый контейнер */
.catalog-wrapper {
  min-height: 100vh;
  background-color: #DCE3EA;
  display: flex;
  flex-direction: column;
}

/* Шапка приложения в соответствии с дизайном XAML */
.catalog-header {
  background-color: #C8D3DF;
  padding: 16px 24px 12px 24px;
  border-bottom-left-radius: 20px;
  border-bottom-right-radius: 20px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.header-top {
  display: flex;
  align-items: center;
  gap: 16px;
}

.header-logo {
  width: 65px;
  height: 65px;
  object-fit: contain;
  border-radius: 12px;
  background: white;
  padding: 4px;
}

/* Овальное поле поиска */
.search-wrapper {
  position: relative;
  flex: 1;
  display: flex;
  align-items: center;
}

.search-input {
  width: 100%;
  height: 38px;
  padding: 0 40px 0 16px;
  border-radius: 20px;
  border: 1px solid #b0bec5;
  outline: none;
  background: white;
  font-size: 14px;
}

.search-icon {
  position: absolute;
  right: 12px;
  width: 18px;
  height: 18px;
  pointer-events: none;
}

/* Селект выбора категории */
.category-select {
  height: 38px;
  min-width: 160px;
  padding: 0 12px;
  border-radius: 8px;
  border: 1px solid #b0bec5;
  background: white;
  font-size: 14px;
  outline: none;
  cursor: pointer;
}

/* Кнопки навигации по макету */
.header-nav {
  display: flex;
  justify-content: center;
  gap: 16px;
}

.nav-btn {
  padding: 8px 32px;
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

/* Область отображения каталога */
.catalog-content {
  padding: 24px;
  flex: 1;
}

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 20px;
}

.empty-msg {
  text-align: center;
  color: #666;
  font-size: 16px;
  margin-top: 40px;
}
</style>
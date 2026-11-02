<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { productsApi } from '@/api/products'
import type { Product } from '@/types/product'
import ProductCard from '@/components/ProductCard.vue'

const allProducts = ref<Product[]>([])
const search = ref('')
const category = ref('Все категории')

onMounted(async () => {
  try {
    const products = await productsApi.getAll()
    
    // Фильтруем: статус не "Удален" И количество на складе больше 0
    allProducts.value = products.filter((p: any) => {
      const isNotDeleted = p.status !== 'Удален' && p.Status !== 'Удален'
      const stock = p.quantity ?? p.Quantity ?? 0
      
      return isNotDeleted && stock > 0
    })
  } catch (e) {
    console.error('Ошибка загрузки товаров:', e)
  }
})

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
    <!-- Панель управления каталогом -->
    <div class="catalog-controls">
      <div class="search-wrapper">
        <input v-model="search" type="text" placeholder="Поиск товаров..." class="search-input" />
        <svg class="search-icon" viewBox="0 0 24 24" fill="none" stroke="#666" stroke-width="2">
          <circle cx="11" cy="11" r="8"></circle>
          <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
        </svg>
      </div>
      <select v-model="category" class="category-select">
        <option v-for="c in categories" :key="c" :value="c">{{ c }}</option>
      </select>
    </div>

    <!-- Основная сетка товаров -->
    <main class="catalog-content">
      <div v-if="filtered.length === 0" class="empty-msg">
        Товары не найдены
      </div>
      <div v-else class="grid">
        <ProductCard v-for="p in filtered" :key="p.id" :product="p" />
      </div>
    </main>
  </div>
</template>

<style scoped>
.catalog-wrapper {
  min-height: 100vh;
  background-color: #DCE3EA;
  display: flex;
  flex-direction: column;
}

.catalog-controls {
  padding: 16px 24px;
  display: flex;
  align-items: center;
  justify-content: center; /* Выравнивание элементов по центру */
  gap: 16px;
  background-color: transparent;
}

.search-wrapper {
  position: relative;
  /* Убрано flex: 1, вместо этого задана ширина 100% с ограничением, чтобы блок корректно центрировался */
  width: 100%; 
  max-width: 600px;
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

.catalog-content {
  padding: 0 24px 24px 24px;
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

/* Адаптив для мобильных устройств, чтобы на узких экранах элементы встали друг над другом */
@media (max-width: 600px) {
  .catalog-controls {
    flex-direction: column;
  }
  .category-select {
    width: 100%;
  }
}
</style>
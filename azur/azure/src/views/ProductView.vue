<template>
  <div class="product-view container">
    <div v-if="loading" class="loading">Загрузка товара...</div>
    <div v-else-if="error" class="error">{{ error }}</div>
    
    <div v-else-if="product" class="product-details">
      <div class="product-image">
        <img :src="product.photo ? (product.photo.startsWith('http') ? product.photo : `http://localhost:5050/${product.photo}`) : '/placeholder.png'" :alt="product.name || product.title" />
      </div>
      
      <div class="product-info">
        <h1>{{ product.name || product.title }}</h1>
        <p class="price">{{ product.price }} ₽</p>
        <p class="description">{{ product.description || 'Описание отсутствует' }}</p>
        
        <div class="actions">
          <input type="number" v-model.number="quantity" min="1" class="quantity-input" />
          <button @click="handleAddToCart" class="add-btn">В корзину</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import { api } from '@/api/axios'
import { cartApi } from '@/api/cart'
import { useAuthStore } from '@/stores/auth'

const route = useRoute()
const auth = useAuthStore()

const product = ref<any>(null)
const loading = ref(true)
const error = ref('')
const quantity = ref(1)


const fetchProduct = async () => {
  try {
    loading.value = true
    
    // Проверяем, передан ли товар через состояние роутера
    const historyState = window.history.state
    if (historyState && historyState.product) {
      product.value = historyState.product
      return
    }

    // Запасной вариант: если открыли по прямой ссылке, берем из localStorage или ставим заглушку
    error.value = 'Информация о товаре недоступна. Откройте его из каталога.'
  } catch (err) {
    error.value = 'Не удалось загрузить информацию о товаре'
    console.error(err)
  } finally {
    loading.value = false
  }

}

// Добавление в корзину с учетом активного заказа
const handleAddToCart = async () => {
  if (!auth.isAuthenticated || !auth.user?.id) {
    alert('Пожалуйста, авторизуйтесь')
    return
  }

  try {
    // 1. Получаем или создаем активный заказ
    let activeOrder
    try {
      activeOrder = await cartApi.getActiveOrder(auth.user.id)
    } catch {
      activeOrder = await cartApi.createOrder(auth.user.id)
    }

    if (!activeOrder || !activeOrder.idOrder) {
      throw new Error('Не удалось определить активный заказ')
    }

    // 2. Добавляем позицию
   const prodId = product.value.id || product.value.idTovar || product.value.Id || product.value.IdTovar
    await cartApi.addItem({
      idOrder: activeOrder.idOrder,
      IdOrder: activeOrder.idOrder,
      idProduct: prodId,
      IdProduct: prodId,
      quantity: quantity.value,
      Quantity: quantity.value,
      price: product.value.price || product.value.Price
    } as any)

    alert('Товар успешно добавлен в корзину!')
  } catch (err) {
    console.error('Ошибка добавления в корзину:', err)
    alert('Не удалось добавить товар в корзину')
  }
}

onMounted(fetchProduct)
</script>

<style scoped>
.product-view {
  max-width: 900px;
  margin: 40px auto;
  padding: 20px;
}
.product-details {
  display: flex;
  gap: 40px;
}
.product-image img {
  width: 300px;
  height: 300px;
  object-fit: cover;
  border-radius: 8px;
}
.product-info {
  display: flex;
  flex-direction: column;
  gap: 15px;
}
.price {
  font-size: 24px;
  font-weight: bold;
  color: #2c3e50;
}
.actions {
  display: flex;
  gap: 15px;
  margin-top: 20px;
}
.quantity-input {
  width: 60px;
  padding: 8px;
  font-size: 16px;
}
.add-btn {
  padding: 10px 20px;
  background-color: #42b883;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 16px;
}
.add-btn:hover {
  background-color: #35495e;
}
</style>
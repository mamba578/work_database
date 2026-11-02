<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRoute } from 'vue-router'
import { api } from '@/api/axios'
import { cartApi } from '@/api/cart'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'

const route = useRoute()
const auth = useAuthStore()
const cartStore = useCartStore()

const product = ref<any>(null)
const loading = ref(true)
const error = ref('')
const quantity = ref(1)

// Вычисляем доступный остаток на складе
const availableStock = computed(() => {
  if (!product.value) return 0
  return product.value.quantity ?? product.value.Quantity ?? 0
})

const fetchProduct = async () => {
  try {
    loading.value = true
    error.value = ''
    
    const historyState = window.history.state
    if (historyState && historyState.product) {
      product.value = historyState.product
      return
    }

    const id = route.params.id
    if (id) {
      const response = await api.get(`/Tovar/GetProduct/${id}`)
      product.value = response.data
      return
    }

    error.value = 'Товар не найден.'
  } catch (err) {
    error.value = 'Не удалось загрузить информацию о товаре'
    console.error(err)
  } finally {
    loading.value = false
  }
}

// Защита от ручного ввода (если пользователь впишет число больше остатка с клавиатуры)
const validateQuantity = () => {
  if (quantity.value > availableStock.value) {
    quantity.value = availableStock.value
  }
  if (quantity.value < 1) {
    quantity.value = 1
  }
}

const handleAddToCart = async () => {
  const userId = auth.user?.id || (auth.user as any)?.Id;
  
  if (!auth.isAuthenticated || !userId) {
    alert('Пожалуйста, авторизуйтесь')
    return
  }

  // Финальная проверка перед отправкой на сервер
  if (quantity.value > availableStock.value) {
    alert(`На складе доступно только ${availableStock.value} шт.`)
    quantity.value = availableStock.value
    return
  }

  try {
    let activeOrder: any
    try {
      activeOrder = await cartApi.getActiveOrder(userId)
    } catch {
      activeOrder = await cartApi.createOrder(userId)
    }

    const orderId = activeOrder?.idOrder ?? activeOrder?.Id ?? activeOrder?.id

    if (!orderId) {
      throw new Error('Не удалось определить активный заказ')
    }

    const prodId = product.value.id ?? product.value.idTovar ?? product.value.Id ?? product.value.IdTovar
    
    await cartApi.addItem({
      idOrder: orderId,
      IdOrder: orderId,
      idProduct: prodId,
      IdProduct: prodId,
      quantity: quantity.value,
      Quantity: quantity.value,
      price: product.value.price ?? product.value.Price
    } as any)
    
    await cartStore.loadCartFromServer()
    
    alert('Товар успешно добавлен в корзину!')
  } catch (err: any) {
    console.error('Ошибка добавления в корзину:', err)
    alert(err.response?.data?.message || 'Не удалось добавить товар в корзину')
  }
}

onMounted(fetchProduct)
</script>

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
        
        <!-- Выводим информацию об остатке -->
        <p class="stock-info" :class="{ empty: availableStock <= 0 }">
          {{ availableStock > 0 ? `В наличии: ${availableStock} шт.` : 'Нет в наличии' }}
        </p>

        <div class="actions">
          <input 
            type="number" 
            v-model.number="quantity" 
            min="1" 
            :max="availableStock"
            @change="validateQuantity"
            class="quantity-input" 
            :disabled="availableStock <= 0"
          />
          <button 
            @click="handleAddToCart" 
            class="add-btn" 
            :disabled="availableStock <= 0"
          >
            {{ availableStock > 0 ? 'В корзину' : 'Распродано' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.product-view {
  max-width: 900px;
  margin: 40px auto;
  padding: 30px;
  background: white;
  border-radius: 16px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.05);
}
.product-details {
  display: flex;
  gap: 40px;
  align-items: flex-start;
}
.product-image img {
  width: 300px;
  height: 300px;
  object-fit: cover;
  border-radius: 12px;
  background: #f7f9fc;
}
.product-info {
  display: flex;
  flex-direction: column;
  gap: 15px;
  flex: 1;
}
h1 {
  margin: 0;
  font-size: 24px;
  color: #1a3a6b;
}
.price {
  font-size: 24px;
  font-weight: bold;
  color: #2c3e50;
  margin: 0;
}
.description {
  color: #555;
  line-height: 1.5;
  margin: 0;
}
.stock-info {
  font-size: 14px;
  color: #38a169;
  font-weight: 600;
  margin: 0;
}
.stock-info.empty {
  color: #e53e3e;
}
.actions {
  display: flex;
  gap: 15px;
  margin-top: 10px;
}
.quantity-input {
  width: 60px;
  padding: 8px;
  font-size: 16px;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  outline: none;
}
.quantity-input:disabled {
  background-color: #f1f5f9;
  color: #94a3b8;
}
.add-btn {
  padding: 10px 24px;
  background-color: #42b883;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 16px;
  transition: background-color 0.2s;
}
.add-btn:hover:not(:disabled) {
  background-color: #35495e;
}
.add-btn:disabled {
  background-color: #cbd5e1;
  cursor: not-allowed;
}
.loading, .error {
  text-align: center;
  font-size: 18px;
  color: #666;
}
</style>
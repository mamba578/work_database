<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'
import { api } from '@/api/axios'

const router = useRouter()
const auth = useAuthStore()
const cart = useCartStore()

const loading = ref(false)
const serverItems = ref<any[]>([])
const activeOrderId = ref<number | null>(null)

// Загружаем элементы корзины из базы данных через OrderItemsController
onMounted(async () => {
  if (!auth.isAuthenticated) return
  
  try {
    const userId = auth.user?.id || (auth.user as any)?.Id
    if (!userId) return

    // 1. Ищем активный заказ пользователя (со статусом корзины)
    // Либо запрашиваем заказы пользователя и берем тот, у которого статус 1 (Корзина)
    const ordersRes = await api.get(`/Order/UserOrders/${userId}`).catch(() => ({ data: [] }))
    const activeOrder = (ordersRes.data || []).find((o: any) => (o.idStatus ?? o.IdStatus) === 1)
    
    const orderId = activeOrder?.id ?? activeOrder?.Id

    if (orderId) {
      activeOrderId.value = orderId
      // 2. Получаем позиции через твой эндпоинт ByOrder/{orderId}
      const itemsRes = await api.get(`/OrderItems/ByOrder/${orderId}`)
      serverItems.value = Array.isArray(itemsRes.data) ? itemsRes.data : []
    }
  } catch (e) {
    console.error('Ошибка при загрузке корзины:', e)
  }
})

// Безопасный список элементов
const validItems = computed(() => {
  return serverItems.value.filter(item => item !== null && item !== undefined)
})

// Общая сумма
const totalPrice = computed(() => {
  return validItems.value.reduce((sum, i) => {
    const p = i.price ?? i.Price ?? 0
    const q = i.quantity ?? i.Quantity ?? 1
    return sum + (p * q)
  }, 0)
})

// Удаление конкретного товара из корзины
async function removeItem(itemId?: number) {
  if (!itemId) return
  try {
    await api.delete(`/OrderItems/DeleteItem/${itemId}`)
    serverItems.value = serverItems.value.filter(i => (i.id ?? i.Id) !== itemId)
  } catch (e) {
    console.error('Ошибка при удалении товара:', e)
  }
}

// Очистка всей корзины (удаляем каждую позицию)
async function clearCartServerSide() {
  try {
    loading.value = true
    for (const item of validItems.value) {
      const itemId = item.id ?? item.Id
      if (itemId) {
        await api.delete(`/OrderItems/DeleteItem/${itemId}`)
      }
    }
    serverItems.value = []
    cart.clearCart()
  } catch (e) {
    console.error('Ошибка при очистке корзины:', e)
  } finally {
    loading.value = false
  }
}

// Переход к оформлению заказа
function handleCheckout() {
  if (validItems.value.length === 0) return

  if (!auth.isAuthenticated) {
    alert('Для оформления заказа необходимо войти в систему')
    router.push('/login')
    return
  }

  router.push('/checkout')
}

// Изменение количества товара (+ / -)
async function updateQuantity(item: any, delta: number) {
  const itemId = item.id ?? item.Id
  const currentQty = item.quantity ?? item.Quantity ?? 1
  const newQty = currentQty + delta

  if (newQty < 1) {
    await removeItem(itemId)
    return
  }

  try {
    // Вызываем твой эндпоинт [HttpPut("UpdateQuantity/{id}")]
    await api.put(`/OrderItems/UpdateQuantity/${itemId}?quantity=${newQty}`)
    item.quantity = newQty
  } catch (e) {
    console.error('Ошибка при обновлении количества:', e)
  }
}

// Формирование ссылки на картинку (согласно твоему селекту TovarPhoto)
function imageUrl(item: any) {
  const foto = item.tovarPhoto || item.TovarPhoto || item.photo || item.Photo || ''
  if (!foto || typeof foto !== 'string') return '/placeholder.png'
  if (foto.startsWith('http')) return foto
  
  const cleanPath = foto.startsWith('/') ? foto.slice(1) : foto
  return cleanPath.toLowerCase().startsWith('images/') 
    ? `http://localhost:5050/${cleanPath}` 
    : `http://localhost:5050/Images/${cleanPath}`
}
</script>

<template>
  <div class="cart-wrapper">
    <header class="catalog-header">
      <div class="header-top">
        <h1 class="header-title">Корзина товаров «Azure»</h1>
      </div>

      <div class="header-nav">
        <button class="nav-btn" @click="router.push('/catalog')">Главная</button>
        <button class="nav-btn active">Корзина ({{ validItems.length }})</button>
        <button class="nav-btn" @click="router.push(auth.isAuthenticated ? '/profile' : '/login')">
          {{ auth.isAuthenticated ? 'Личный кабинет' : 'Войти' }}
        </button>
      </div>
    </header>

    <main class="cart-content">
      <div class="cart-card">
        <h2>Состав заказа</h2>

        <!-- Если корзина пуста -->
        <div v-if="validItems.length === 0" class="empty-cart">
          <p>Ваша корзина пока пуста</p>
          <button class="btn-catalog" @click="router.push('/catalog')">
            Перейти в каталог
          </button>
        </div>

        <!-- Список товаров в корзине -->
        <div v-else class="cart-body">
          <div class="cart-items">
            <div 
              v-for="item in validItems" 
              :key="item.id ?? item.Id" 
              class="cart-item-card"
            >
              <div class="item-img-wrap">
                <img :src="imageUrl(item)" alt="Товар" @error="(e: any) => e.target.src = '/placeholder.png'" />
              </div>

              <div class="item-details">
                <h3>{{ item.tovarName ?? item.TovarName ?? ('Товар #' + (item.idTovar ?? item.IdTovar)) }}</h3>
                <p class="item-price-line">Цена: <span>{{ item.price ?? item.Price ?? 0 }} ₽</span></p>
                
                <div class="quantity-block">
                  <span class="q-label">Количество:</span>
                  <button class="q-btn minus" @click="updateQuantity(item, -1)">-</button>
                  <span class="q-value">{{ item.quantity ?? item.Quantity ?? 1 }}</span>
                  <button class="q-btn plus" @click="updateQuantity(item, 1)">+</button>
                </div>

                <p class="item-sum-line">Сумма: <strong>{{ ((item.price ?? item.Price ?? 0) * (item.quantity ?? item.Quantity ?? 1)).toFixed(2) }} ₽</strong></p>
              </div>

              <button class="btn-remove-card" @click="removeItem(item.id ?? item.Id)" title="Удалить">
                ✕
              </button>
            </div>
          </div>

          <div class="cart-footer">
            <div class="total-info">
              <span>Итого к оплате:</span>
              <span class="total-price">
                {{ totalPrice.toFixed(2) }} ₽
              </span>
            </div>

            <div class="action-buttons">
              <button class="btn-clear" @click="clearCartServerSide" :disabled="loading">
                Очистить корзину
              </button>
              <button class="btn-checkout" @click="handleCheckout" :disabled="loading">
                Оформить заказ
              </button>
            </div>
          </div>
        </div>
      </div> 
    </main>
  </div>
</template>

<style scoped>
.cart-wrapper {
  min-height: 100vh;
  background-color: #DCE3EA;
  display: flex;
  flex-direction: column;
}
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
.header-title {
  font-size: 22px;
  color: #1a3a6b;
  margin: 0;
}
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
.cart-content {
  padding: 24px;
  flex: 1;
  max-width: 800px;
  margin: 0 auto;
  width: 100%;
}
.cart-card {
  background: white;
  border-radius: 16px;
  padding: 28px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
}
.cart-card h2 {
  margin-top: 0;
  color: #1a3a6b;
  font-size: 22px;
  margin-bottom: 20px;
  border-bottom: 2px solid #eef2f6;
  padding-bottom: 12px;
}
.empty-cart {
  text-align: center;
  padding: 40px 0;
}
.empty-cart p {
  color: #666;
  font-size: 16px;
  margin-bottom: 20px;
}
.btn-catalog {
  padding: 10px 24px;
  background-color: #6F8AA6;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
}
.btn-catalog:hover {
  background-color: #557290;
}
.cart-items {
  display: flex;
  flex-direction: column;
  gap: 12px;
  margin-bottom: 24px;
}
.cart-item-card {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: white;
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 16px;
  margin-bottom: 12px;
  position: relative;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.02);
  gap: 16px;
}
.item-img-wrap {
  width: 80px;
  height: 80px;
  flex-shrink: 0;
  border-radius: 8px;
  overflow: hidden;
  border: 1px solid #eee;
  background: #f7f9fc;
  display: flex;
  align-items: center;
  justify-content: center;
}
.item-img-wrap img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.item-details {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 6px;
}
.item-details h3 {
  margin: 0;
  font-size: 16px;
  color: #1a3a6b;
  font-weight: 600;
}
.item-price-line, .item-sum-line {
  margin: 0;
  font-size: 14px;
  color: #555;
}
.quantity-block {
  display: flex;
  align-items: center;
  gap: 10px;
  margin: 4px 0;
}
.q-label {
  font-size: 14px;
  color: #555;
}
.q-btn {
  background: transparent;
  border: none;
  font-size: 18px;
  font-weight: bold;
  cursor: pointer;
  padding: 0 6px;
  transition: opacity 0.2s;
}
.q-btn.minus {
  color: #e53e3e;
}
.q-btn.plus {
  color: #38a169;
}
.q-btn:hover {
  opacity: 0.7;
}
.q-value {
  font-weight: bold;
  font-size: 15px;
  min-width: 16px;
  text-align: center;
}
.btn-remove-card {
  background: transparent;
  border: none;
  color: #e53e3e;
  font-size: 20px;
  font-weight: bold;
  cursor: pointer;
  padding: 4px 8px;
  transition: opacity 0.2s;
  flex-shrink: 0;
}
.btn-remove-card:hover {
  opacity: 0.7;
}
.cart-footer {
  border-top: 2px solid #eef2f6;
  padding-top: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.total-info {
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 18px;
  font-weight: bold;
  color: #1a3a6b;
}
.total-price {
  font-size: 22px;
  color: #2b6cb0;
}
.action-buttons {
  display: flex;
  justify-content: space-between;
  gap: 16px;
}
.btn-clear {
  padding: 12px 20px;
  background: transparent;
  border: 1px solid #e53e3e;
  color: #e53e3e;
  border-radius: 8px;
  font-weight: 600;
  cursor: pointer;
}
.btn-clear:hover {
  background: #fff5f5;
}
.btn-checkout {
  padding: 12px 28px;
  background: #6F8AA6;
  color: white;
  border: none;
  border-radius: 8px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
}
.btn-checkout:hover:not(:disabled) {
  background: #357ec6;
}
.btn-checkout:disabled {
  background: #bdbdbd;
  cursor: not-allowed;
}
</style>
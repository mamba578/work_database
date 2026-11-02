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

onMounted(async () => {
  if (!auth.isAuthenticated) return
  
  try {
    const userId = auth.user?.id || (auth.user as any)?.Id
    if (!userId) return

    const ordersRes = await api.get(`/Order/UserOrders/${userId}`).catch(() => ({ data: [] }))
    const activeOrder = (ordersRes.data || []).find((o: any) => (o.idStatus ?? o.IdStatus) === 1)
    
    const orderId = activeOrder?.id ?? activeOrder?.Id

    if (orderId) {
      activeOrderId.value = orderId
      const itemsRes = await api.get(`/OrderItems/ByOrder/${orderId}`)
      const rawItems = Array.isArray(itemsRes.data) ? itemsRes.data : []

      // Подгружаем каталог товаров, чтобы узнать реальный остаток на складе
      let allTovars: any[] = []
      try {
        const tovarsRes = await api.get('/Tovar/AllProducts')
        allTovars = Array.isArray(tovarsRes.data) ? tovarsRes.data : []
      } catch (err) {
        console.warn('Не удалось подгрузить каталог товаров', err)
      }

      // Связываем элементы корзины с каталогом
      serverItems.value = rawItems.map((item: any) => {
        const tovarId = item.id_tovar || item.idTovar || item.idProduct
        const matchedTovar = allTovars.find((t: any) => (t.id ?? t.Id) === tovarId) || {}

        return {
          ...item,
          // Сохраняем реальное количество на складе (если не найдено, ставим 0)
          availableStock: matchedTovar.quantity ?? matchedTovar.Quantity ?? 0
        }
      })

      cart.items = serverItems.value 
    }
  } catch (e) {
    console.error('Ошибка при загрузке корзины:', e)
  }
})

const validItems = computed(() => {
  return serverItems.value.filter(item => item !== null && item !== undefined)
})

const totalPrice = computed(() => {
  return validItems.value.reduce((sum, i) => {
    const p = i.price ?? i.Price ?? 0
    const q = i.quantity ?? i.Quantity ?? 1
    return sum + (p * q)
  }, 0)
})

async function removeItem(itemId?: number) {
  if (!itemId) return
  try {
    await api.delete(`/OrderItems/DeleteItem/${itemId}`)
    serverItems.value = serverItems.value.filter(i => (i.id ?? i.Id) !== itemId)
    cart.items = serverItems.value 
  } catch (e) {
    console.error('Ошибка при удалении товара:', e)
  }
}

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

function handleCheckout() {
  if (validItems.value.length === 0) return

  if (!auth.isAuthenticated) {
    alert('Для оформления заказа необходимо войти в систему')
    router.push('/login')
    return
  }

  router.push('/checkout')
}

// Получаем остаток из ранее замапленного свойства
function getAvailableStock(item: any): number {
  return item.availableStock ?? 0; 
}

async function updateQuantity(item: any, delta: number) {
  const itemId = item.id ?? item.Id
  const currentQty = item.quantity ?? item.Quantity ?? 1
  const newQty = currentQty + delta

  // Ограничение: нельзя добавить больше, чем есть на складе
  const maxStock = getAvailableStock(item)
  if (newQty > maxStock) {
    alert(`На складе доступно только ${maxStock} шт.`)
    return
  }

  if (newQty < 1) {
    await removeItem(itemId)
    return
  }

  try {
    await api.put(`/OrderItems/UpdateQuantity/${itemId}?quantity=${newQty}`)
    item.quantity = newQty
  } catch (e) {
    console.error('Ошибка при обновлении количества:', e)
  }
}

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
    <main class="cart-content">
      <div class="cart-card">
        <h2>Состав заказа</h2>

        <div v-if="validItems.length === 0" class="empty-cart">
          <p>Ваша корзина пока пуста</p>
          <button class="btn-catalog" @click="router.push('/catalog')">
            Перейти в каталог
          </button>
        </div>

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
                  <!-- Кнопка + блокируется при достижении лимита склада -->
                  <button 
                    class="q-btn plus" 
                    @click="updateQuantity(item, 1)"
                    :disabled="(item.quantity ?? item.Quantity ?? 1) >= getAvailableStock(item)"
                    :title="(item.quantity ?? item.Quantity ?? 1) >= getAvailableStock(item) ? 'Достигнут максимум на складе' : ''"
                  >+</button>
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

.q-btn:hover:not(:disabled) {
  opacity: 0.7;
}

.q-btn:disabled {
  color: #cbd5e1;
  cursor: not-allowed;
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
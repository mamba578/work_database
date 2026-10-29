<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'
import { api } from '@/api/axios'
import { cartApi } from '@/api/cart'

const router = useRouter()
const auth = useAuthStore()
const cart = useCartStore()

// Данные формы
const points = ref<any[]>([])
const pointSearchQuery = ref('')
const selectedPointId = ref<number | null>(null)
const showPointsDropdown = ref(false)

const paymentMethod = ref('cash') // 'cash' или 'card'
const sendEmailReceipt = ref(true)
const userEmail = ref(auth.user?.email || '')
const loading = ref(false)
const orderItems = ref<any[]>([])
const activeOrderId = ref<number | null>(null)

onMounted(async () => {
  if (!auth.isAuthenticated) return
  try {
    const userId = auth.user?.id || (auth.user as any)?.Id
    if (!userId) return

    // 1. Загружаем активный заказ и его позиции
    const activeOrder: any = await cartApi.getActiveOrder(userId)
    const orderId = activeOrder?.idOrder ?? activeOrder?.Id ?? activeOrder?.id

    if (orderId) {
      activeOrderId.value = orderId
      const items: any = await cartApi.getByOrder(orderId)
      
      // 2. Подгружаем общий список товаров из TovarController, чтобы точно сопоставить название и картинку по ID
      let allTovars: any[] = []
      try {
        const tovarsRes = await api.get('/Tovar/AllProducts')
        allTovars = Array.isArray(tovarsRes.data) ? tovarsRes.data : []
      } catch (err) {
        console.warn('Не удалось подгрузить каталог товаров', err)
      }

      // 3. Мапим элементы заказа, связывая их с каталогом товаров
      orderItems.value = (Array.isArray(items) ? items : []).map((item: any) => {
        const tovarId = item.id_tovar || item.idTovar || item.idProduct
        const matchedTovar = allTovars.find((t: any) => (t.id ?? t.Id) === tovarId) || {}

        const title = matchedTovar.title || matchedTovar.Title || item.title || ('Товар #' + tovarId)
        const photo = matchedTovar.photo || matchedTovar.Photo || item.photo || ''
        const price = item.price ?? matchedTovar.price ?? matchedTovar.Price ?? 0

        return {
          ...item,
          idItem: item.id ?? item.Id,
          idTovar: tovarId,
          quantity: item.quantity ?? item.Quantity ?? 1,
          price: price,
          title: title,
          photo: photo
        }
      })
    }

    // 4. Загружаем список пунктов выдачи
    const pointsRes = await api.get('/Point/AllPoints')
    points.value = Array.isArray(pointsRes.data) ? pointsRes.data : []
  } catch (e) {
    console.error('Ошибка при инициализации страницы оформления:', e)
  }
})

// Фильтрация пунктов выдачи по введенному тексту
const filteredPoints = computed(() => {
  if (!pointSearchQuery.value.trim()) return points.value
  const q = pointSearchQuery.value.toLowerCase()
  return points.value.filter((p: any) => {
    const addressStr = p.address || p.Address || ''
    const cityStr = p.city || p.City || ''
    return addressStr.toLowerCase().includes(q) || cityStr.toLowerCase().includes(q)
  })
})

function selectPoint(point: any) {
  const addressStr = point.address || point.Address || ''
  pointSearchQuery.value = addressStr
  selectedPointId.value = point.id ?? point.Id
  showPointsDropdown.value = false
}

const totalPrice = computed(() => {
  return orderItems.value.reduce((sum, i: any) => {
    const p = i.price ?? 0
    const q = i.quantity ?? 1
    return sum + p * q
  }, 0)
})

const totalCount = computed(() => {
  return orderItems.value.reduce((sum, i: any) => sum + (i.quantity ?? 1), 0)
})

const estimatedDeliveryDate = computed(() => {
  const d = new Date()
  d.setDate(d.getDate() + 3)
  return d.toLocaleDateString('ru-RU')
})

// Функция для формирования ссылки на картинку
function imageUrl(photo: string | undefined) {
  if (!photo || typeof photo !== 'string') return '/placeholder.png'
  if (photo.startsWith('http')) return photo
  
  const cleanPath = photo.startsWith('/') ? photo.slice(1) : photo
  return cleanPath.toLowerCase().startsWith('images/') 
    ? `http://localhost:5050/${cleanPath}` 
    : `http://localhost:5050/Images/${cleanPath}`
}

async function submitOrder() {
  if (!selectedPointId.value) {
    alert('Пожалуйста, выберите пункт выдачи из выпадающего списка')
    return
  }

  loading.value = true
  try {
    const orderId = activeOrderId.value
    if (orderId) {
      await api.put(`/Order/UpdateStatus/${orderId}`, null, {
        params: {
          newStatusId: 2, // 2 — Оформлен / Оплачен
          idPoint: selectedPointId.value,
          paymentType: paymentMethod.value === 'card' ? 'Онлайн' : 'При получении'
        }
      })
    }

    alert('Заказ успешно оформлен! Чек отправлен на вашу почту.')
    cart.clearCart()
    router.push('/profile')
  } catch (e: any) {
    console.error('Ошибка при оформлении заказа:', e)
    alert(e.response?.data?.message || 'Не удалось оформить заказ. Попробуйте позже.')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="checkout-wrapper">
    <header class="catalog-header">
      <div class="header-top">
        <h1 class="header-title">Оформление заказа «Azure»</h1>
      </div>
      <div class="header-nav">
        <button class="nav-btn" @click="router.push('/cart')">Назад в корзину</button>
      </div>
    </header>

    <main class="checkout-content-grid">
      <!-- Левая колонка: Ваш заказ -->
      <section class="order-left-column">
        <h2 class="section-title">Ваш заказ</h2>
        
        <div class="order-items-list">
          <div v-for="(item, index) in orderItems" :key="index" class="order-item-card">
            <div class="item-img-wrap">
              <img 
                :src="imageUrl(item.photo)" 
                alt="Товар" 
                @error="(e: any) => e.target.src = '/placeholder.png'" 
              />
            </div>
            <div class="item-info">
              <h3>{{ item.title }}</h3>
              <p class="item-price">Цена: {{ item.price ?? 0 }} ₽</p>
              <p class="item-qty">Количество: {{ item.quantity ?? 1 }}</p>
              <p class="item-sum">
                Сумма: {{ ((item.price ?? 0) * (item.quantity ?? 1)).toFixed(2) }} ₽
              </p>
            </div>
          </div>
        </div>
      </section>

      <!-- Правая колонка: Оформление -->
      <section class="order-right-column">
        <div class="checkout-box">
          <h2 class="section-title">Оформление</h2>

          <div class="form-group autocomplete-container">
            <label>Пункт выдачи</label>
            <input 
              type="text" 
              v-model="pointSearchQuery" 
              @focus="showPointsDropdown = true"
              placeholder="Начните вводить адрес пункта..." 
              class="form-input"
            />
            
            <div v-if="showPointsDropdown && filteredPoints.length > 0" class="dropdown-list">
              <div 
                v-for="point in filteredPoints" 
                :key="point.id ?? point.Id" 
                class="dropdown-item"
                @click="selectPoint(point)"
              >
                {{ point.address ?? point.Address }} <span v-if="point.city || point.City">({{ point.city ?? point.City }})</span>
              </div>
            </div>
          </div>

          <div class="form-group">
            <label>Способ оплаты</label>
            <div class="radio-group">
              <label class="radio-label">
                <input type="radio" value="cash" v-model="paymentMethod" />
                Оплата при получении
              </label>
              <label class="radio-label">
                <input type="radio" value="card" v-model="paymentMethod" />
                Оплата сразу (картой онлайн)
              </label>
            </div>
          </div>

          <div class="form-group checkbox-group">
            <label class="checkbox-label">
              <input type="checkbox" v-model="sendEmailReceipt" />
              Отправить чек на почту ({{ userEmail || 'не указана' }})
            </label>
          </div>

          <div class="order-summary-card">
            <div class="summary-line">Товаров: <span>{{ totalCount }}</span></div>
            <div class="summary-line total">Итого: <span>{{ totalPrice.toFixed(2) }} ₽</span></div>
            <div class="summary-line delivery">Примерная доставка: <span>{{ estimatedDeliveryDate }}</span></div>
          </div>

          <button class="btn-order" @click="submitOrder" :disabled="loading">
            {{ loading ? 'Обработка...' : 'Заказать' }}
          </button>
        </div>
      </section>
    </main>
  </div>
</template>

<style scoped>
.checkout-wrapper {
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
  justify-content: flex-start;
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
}
.nav-btn:hover {
  background-color: #557290;
}
.checkout-content-grid {
  padding: 24px;
  flex: 1;
  max-width: 1000px;
  margin: 0 auto;
  width: 100%;
  display: grid;
  grid-template-columns: 1.2fr 1fr;
  gap: 24px;
}
@media (max-width: 768px) {
  .checkout-content-grid {
    grid-template-columns: 1fr;
  }
}
.section-title {
  font-size: 22px;
  color: #1a3a6b;
  margin-top: 0;
  margin-bottom: 16px;
  font-weight: 700;
}
.order-left-column {
  display: flex;
  flex-direction: column;
}
.order-items-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.order-item-card {
  display: flex;
  align-items: center;
  background: white;
  border-radius: 14px;
  padding: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.03);
  gap: 16px;
  border: 1px solid #e2e8f0;
}
.item-img-wrap {
  width: 70px;
  height: 70px;
  flex-shrink: 0;
  border-radius: 8px;
  overflow: hidden;
  background: #f7f9fc;
  border: 1px solid #eee;
  display: flex;
  align-items: center;
  justify-content: center;
}
.item-img-wrap img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}
.item-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.item-info h3 {
  margin: 0;
  font-size: 15px;
  color: #1a3a6b;
  font-weight: 600;
}
.item-price, .item-qty, .item-sum {
  margin: 0;
  font-size: 13px;
  color: #444;
}
.checkout-box {
  background: white;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
  position: relative;
}
.form-group label {
  font-weight: 600;
  font-size: 13px;
  color: #333;
}
.form-input {
  padding: 10px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  font-size: 14px;
  outline: none;
  transition: border-color 0.2s;
}
.form-input:focus {
  border-color: #6F8AA6;
}
.dropdown-list {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background: white;
  border: 1px solid #cbd5e1;
  border-radius: 8px;
  max-height: 180px;
  overflow-y: auto;
  z-index: 10;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  margin-top: 4px;
}
.dropdown-item {
  padding: 10px 12px;
  font-size: 13px;
  cursor: pointer;
  color: #333;
  border-bottom: 1px solid #f1f5f9;
}
.dropdown-item:hover {
  background-color: #f8fafc;
  color: #1a3a6b;
}
.radio-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-top: 4px;
}
.radio-label, .checkbox-label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  cursor: pointer;
  color: #444;
}
.order-summary-card {
  background: #f7f9fc;
  border-radius: 12px;
  padding: 14px;
  display: flex;
  flex-direction: column;
  gap: 6px;
  border: 1px solid #e2e8f0;
}
.summary-line {
  font-size: 14px;
  color: #555;
  display: flex;
  justify-content: space-between;
}
.summary-line.total {
  font-weight: bold;
  font-size: 16px;
  color: #1a3a6b;
  border-top: 1px solid #e2e8f0;
  padding-top: 6px;
  margin-top: 2px;
}
.summary-line.delivery {
  font-size: 12px;
  color: #666;
}
.btn-order {
  width: 100%;
  padding: 12px;
  background-color: #6F8AA6;
  color: white;
  border: none;
  border-radius: 10px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
}
.btn-order:hover:not(:disabled) {
  background-color: #557290;
}
.btn-order:disabled {
  background: #bdbdbd;
  cursor: not-allowed;
}
</style>
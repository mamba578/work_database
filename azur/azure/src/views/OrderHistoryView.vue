<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import {api} from '@/api/axios'

interface OrderItem {
  idProduct?: number
  productName?: string
  title?: string
  quantity: number
  price: number
}

interface Order {
  idOrder?: number
  id?: number
  orderDate: string
  status: string
  totalPrice: number
  items?: OrderItem[]
}

const router = useRouter()
const auth = useAuthStore()

const orders = ref<Order[]>([])
const loading = ref(true)
const error = ref('')
const expandedOrderId = ref<number | null>(null)

onMounted(async () => {
  if (!auth.isAuthenticated) {
    router.push('/login')
    return
  }

  const userId = auth.user?.idUser || auth.user?.id

  try {
    const res = await api.get(`/orders/user/${userId}`)
    orders.value = res.data
  } catch (e: any) {
    console.error('Ошибка при загрузке истории заказов:', e)
    error.value = 'Не удалось загрузить историю заказов. Проверьте подключение к серверу.'
  } finally {
    loading.value = false
  }
})

function toggleOrderDetails(id: number | undefined) {
  if (!id) return
  expandedOrderId.value = expandedOrderId.value === id ? null : id
}

function formatDate(dateStr: string): string {
  if (!dateStr) return '—'
  const date = new Date(dateStr)
  return date.toLocaleDateString('ru-RU', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

function getStatusClass(status: string): string {
  switch (status?.toLowerCase()) {
    case 'выполнен':
    case 'доставлен':
    case 'оплачен':
      return 'status-completed'
    case 'отменен':
    case 'отменён':
      return 'status-cancelled'
    default:
      return 'status-pending'
  }
}
</script>

<template>
  <div class="orders-wrapper">
    <!-- Фирменная шапка Azure -->
    <header class="catalog-header">
      <div class="header-top">
        <h1 class="header-title">История заказов «Azure»</h1>
      </div>

      <!-- Кнопки навигации -->
      <div class="header-nav">
        <button class="nav-btn" @click="router.push('/catalog')">Главная</button>
        <button class="nav-btn" @click="router.push('/cart')">Корзина</button>
        <button class="nav-btn" @click="router.push('/profile')">Личный кабинет</button>
        <button class="nav-btn active">История заказов</button>
      </div>
    </header>

    <!-- Основной контент -->
    <main class="orders-content">
      <div class="orders-card">
        <h2>Мои покупки</h2>

        <!-- Сообщение об ошибке -->
        <div v-if="error" class="msg-box error-msg">
          {{ error }}
        </div>

        <!-- Индикатор загрузки -->
        <div v-else-if="loading" class="msg-box loading-msg">
          Загрузка истории заказов...
        </div>

        <!-- Если нет заказов -->
        <div v-else-if="orders.length === 0" class="msg-box empty-msg">
          <p>У вас пока нет оформленных заказов.</p>
          <button class="btn-catalog" @click="router.push('/catalog')">
            Перейти в каталог
          </button>
        </div>

        <!-- Список заказов -->
        <div v-else class="orders-list">
          <div
            v-for="order in orders"
            :key="order.idOrder || order.id"
            class="order-item"
          >
            <!-- Заголовок карточки заказа -->
            <div
              class="order-header"
              @click="toggleOrderDetails(order.idOrder || order.id)"
            >
              <div class="order-main-info">
                <span class="order-id">Заказ №{{ order.idOrder || order.id }}</span>
                <span class="order-date">{{ formatDate(order.orderDate) }}</span>
              </div>

              <div class="order-side-info">
                <span class="order-status" :class="getStatusClass(order.status)">
                  {{ order.status || 'В обработке' }}
                </span>
                <span class="order-price">{{ order.totalPrice }} ₽</span>
                <span class="toggle-icon">
                  {{ expandedOrderId === (order.idOrder || order.id) ? '▲' : '▼' }}
                </span>
              </div>
            </div>

            <!-- Раскрывающийся состав заказа -->
            <div
              v-if="expandedOrderId === (order.idOrder || order.id)"
              class="order-details"
            >
              <h4>Состав заказа:</h4>
              <div v-if="order.items && order.items.length > 0" class="items-table">
                <div
                  v-for="(item, idx) in order.items"
                  :key="idx"
                  class="table-row"
                >
                  <span class="item-name">
                    {{ item.title || item.productName || 'Товар для творчества' }}
                  </span>
                  <span class="item-qty">{{ item.quantity }} шт.</span>
                  <span class="item-sum">{{ item.price * item.quantity }} ₽</span>
                </div>
              </div>
              <div v-else class="no-items">
                Состав заказа не указан.
              </div>
            </div>
          </div>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
/* Полноэкранный контейнер с фоном #DCE3EA */
.orders-wrapper {
  min-height: 100vh;
  background-color: #DCE3EA;
  display: flex;
  flex-direction: column;
}

/* Фирменная шапка Azure */
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
  width: 60px;
  height: 60px;
  object-fit: contain;
  border-radius: 12px;
  background: white;
  padding: 4px;
}

.header-title {
  font-size: 22px;
  color: #1a3a6b;
  margin: 0;
}

/* Кнопки навигации */
.header-nav {
  display: flex;
  justify-content: center;
  gap: 16px;
  flex-wrap: wrap;
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

/* Контент */
.orders-content {
  padding: 24px;
  flex: 1;
  max-width: 850px;
  margin: 0 auto;
  width: 100%;
}

.orders-card {
  background: white;
  border-radius: 16px;
  padding: 28px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
}

.orders-card h2 {
  margin-top: 0;
  color: #1a3a6b;
  font-size: 22px;
  margin-bottom: 20px;
  border-bottom: 2px solid #eef2f6;
  padding-bottom: 12px;
}

/* Сообщения */
.msg-box {
  text-align: center;
  padding: 40px 0;
  font-size: 16px;
  color: #666;
}

.error-msg {
  color: #dc3545;
}

.btn-catalog {
  margin-top: 16px;
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

/* Карточка заказа */
.orders-list {
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.order-item {
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  background: #fdfdfd;
  overflow: hidden;
  transition: border-color 0.2s;
}

.order-item:hover {
  border-color: #6F8AA6;
}

.order-header {
  padding: 16px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  cursor: pointer;
  user-select: none;
  background: #fafbfc;
}

.order-main-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.order-id {
  font-weight: bold;
  font-size: 16px;
  color: #1a3a6b;
}

.order-date {
  font-size: 13px;
  color: #777;
}

.order-side-info {
  display: flex;
  align-items: center;
  gap: 16px;
}

/* Статусы заказов */
.order-status {
  font-size: 13px;
  padding: 4px 10px;
  border-radius: 12px;
  font-weight: 600;
}

.status-pending {
  background: #fef3c7;
  color: #d97706;
}

.status-completed {
  background: #d1fae5;
  color: #059669;
}

.status-cancelled {
  background: #fee2e2;
  color: #dc2626;
}

.order-price {
  font-size: 17px;
  font-weight: bold;
  color: #2b6cb0;
}

.toggle-icon {
  font-size: 12px;
  color: #888;
}

/* Детали заказа */
.order-details {
  padding: 16px 20px;
  background: white;
  border-top: 1px dashed #e2e8f0;
}

.order-details h4 {
  margin: 0 0 12px 0;
  font-size: 14px;
  color: #555;
}

.items-table {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.table-row {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
  padding: 6px 0;
  border-bottom: 1px solid #f1f5f9;
}

.item-name {
  flex: 1;
  color: #333;
}

.item-qty {
  width: 80px;
  text-align: center;
  color: #666;
}

.item-sum {
  font-weight: 600;
  color: #1a3a6b;
}

.no-items {
  font-size: 13px;
  color: #888;
}
</style>

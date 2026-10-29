<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { api } from '@/api/axios'

interface OrderItem {
  id: number
  idTovar: number
  quantity: number
  price: number
}

interface Order {
  id: number
  orderDate: string
  deliveryDate: string
  totalPrice: number
  paymentType: string
  idStatus: number
  idPoint: number
  orderItems?: OrderItem[]
}

const router = useRouter()
const auth = useAuthStore()

const user = ref(auth.user || JSON.parse(localStorage.getItem('user') || 'null'))
const orders = ref<Order[]>([])
const loadingOrders = ref(true)

// Состояние редактирования профиля
const isEditing = ref(false)
const loadingUpdate = ref(false)
const form = ref({
  name: '',
  surname: '',
  patronymic: '',
  login: '',
  email: '',
  password: ''
})

onMounted(async () => {
  if (!auth.isAuthenticated && !user.value) {
    router.push('/login')
    return
  }

  // Заполняем форму текущими данными
  initForm()

  try {
    const userId = user.value?.id || user.value?.Id
    if (!userId) return

  const res = await api.get(`/Order/UserOrders/${userId}`)
  orders.value = res.data
  } catch (e) {
    console.error('Ошибка при загрузке истории заказов:', e)
  } finally {
    loadingOrders.value = false
  }
})

function initForm() {
  const u = user.value
  if (u) {
    form.value.name = u.name || u.Name || ''
    form.value.surname = u.surname || u.Surname || ''
    form.value.patronymic = u.patronymic || u.Patronymic || ''
    form.value.login = u.login || u.Login || ''
    form.value.email = u.email || u.Email || ''
    form.value.password = u.password || u.Password || ''
  }
}

async function handleUpdate() {
  loadingUpdate.value = true
  try {
    const userId = user.value?.id || user.value?.Id
    if (!userId) return

    // Передаем полную структуру объекта, включая существующие роли, статус и ID
    const updatedPayload = {
      id: userId,
      name: form.value.name,
      surname: form.value.surname,
      patronymic: form.value.patronymic,
      login: form.value.login,
      email: form.value.email,
      password: form.value.password,
      role: user.value?.role || user.value?.Role || 'Клиент',
      status: user.value?.status || user.value?.Status || 'Активный'
    }

    const response = await api.put(`/User/UpdateUser?id=${userId}`, updatedPayload)

    if (response.data) {
      user.value = { ...user.value, ...response.data }
      auth.user = user.value
      localStorage.setItem('user', JSON.stringify(user.value))
    }

    alert('Профиль успешно обновлен!')
    isEditing.value = false
  } catch (e: any) {
    console.error('Ошибка при обновлении профиля:', e)
    alert(e.response?.data?.message || 'Не удалось обновить профиль.')
  } finally {
    loadingUpdate.value = false
  }
}

function logout() {
  auth.logout?.()
  localStorage.clear()
  router.push('/login')
}
</script>

<template>
  <div class="user-wrapper">
    <header class="catalog-header">
      <div class="header-top">
        <h1 class="header-title">Личный кабинет «Azure»</h1>
      </div>
      <div class="header-nav">
        <button class="nav-btn" @click="router.push('/catalog')">Главная</button>
        <button class="nav-btn" @click="router.push('/cart')">Корзина</button>
        <button class="nav-btn active">Личный кабинет</button>
      </div>
    </header>

    <main class="user-content">
      <div class="profile-grid">
        <!-- Левая колонка: Профиль с возможностью редактирования -->
        <section class="profile-card">
          <div class="avatar-container">
            <div class="avatar-placeholder">
              {{ (user?.login || user?.Login)?.charAt(0).toUpperCase() || 'U' }}
            </div>
            <h2>{{ user?.login || user?.Login || 'Пользователь' }}</h2>
            <span class="role-badge">{{ (user?.role || user?.Role) === 'Администратор' ? 'Администратор' : 'Клиент' }}</span>
          </div>

          <!-- Режим просмотра данных -->
          <div v-if="!isEditing" class="info-list">
            <div class="info-item">
              <span class="info-label">Имя:</span>
              <span class="info-value">{{ user?.name || user?.Name || 'Не указано' }}</span>
            </div>
            <div class="info-item">
              <span class="info-label">Фамилия:</span>
              <span class="info-value">{{ user?.surname || user?.Surname || 'Не указана' }}</span>
            </div>
            <div class="info-item">
              <span class="info-label">Email:</span>
              <span class="info-value">{{ user?.email || user?.Email || 'Не указан' }}</span>
            </div>
            <button class="edit-btn" @click="isEditing = true">Редактировать профиль</button>
          </div>

          <!-- Режим редактирования данных -->
          <div v-else class="edit-form-list">
            <div class="form-group">
              <label>Имя</label>
              <input type="text" v-model="form.name" class="form-input" />
            </div>
            <div class="form-group">
              <label>Фамилия</label>
              <input type="text" v-model="form.surname" class="form-input" />
            </div>
            <div class="form-group">
              <label>Отчество</label>
              <input type="text" v-model="form.patronymic" class="form-input" />
            </div>
            <div class="form-group">
              <label>Логин</label>
              <input type="text" v-model="form.login" class="form-input" />
            </div>
            <div class="form-group">
              <label>Email</label>
              <input type="email" v-model="form.email" class="form-input" />
            </div>
            <div class="form-group">
              <label>Пароль</label>
              <input type="password" v-model="form.password" class="form-input" />
            </div>

            <div class="form-actions">
              <button class="save-btn" @click="handleUpdate" :disabled="loadingUpdate">
                {{ loadingUpdate ? 'Сохранение...' : 'Сохранить' }}
              </button>
              <button class="cancel-btn" @click="isEditing = false; initForm()">Отмена</button>
            </div>
          </div>

          <button class="logout-btn" @click="logout">Выйти из аккаунта</button>
        </section>

        <!-- Правая колонка: История заказов -->
        <section class="orders-card">
          <h2>История заказов</h2>

          <div v-if="loadingOrders" class="status-msg">Загрузка заказов...</div>
          <div v-else-if="orders.length === 0" class="status-msg">У вас пока нет оформленных заказов.</div>

          <div v-else class="orders-list">
            <div v-for="order in orders" :key="order.id" class="order-item">
              <div class="order-header">
                <span class="order-id">Заказ №{{ order.id }}</span>
                <span class="order-status">Статус ID: {{ order.idStatus }}</span>
              </div>
              <div class="order-details">
                <span>Дата: {{ new Date(order.orderDate).toLocaleDateString('ru-RU') }}</span>
                <span class="order-price">Сумма: {{ order.totalPrice }} ₽</span>
              </div>
            </div>
          </div>
        </section>
      </div>
    </main>
  </div>
</template>

<style scoped>
.user-wrapper { min-height: 100vh; background-color: #DCE3EA; display: flex; flex-direction: column; }
.catalog-header { background-color: #C8D3DF; padding: 16px 24px 12px 24px; border-bottom-left-radius: 20px; border-bottom-right-radius: 20px; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05); display: flex; flex-direction: column; gap: 12px; }
.header-top { display: flex; align-items: center; gap: 16px; }
.header-title { font-size: 22px; color: #1a3a6b; margin: 0; }
.header-nav { display: flex; justify-content: center; gap: 16px; }
.nav-btn { padding: 8px 32px; border-radius: 18px; border: none; background-color: #6F8AA6; color: white; font-size: 15px; font-weight: 600; cursor: pointer; transition: all 0.2s; box-shadow: 0 2px 6px rgba(0, 0, 0, 0.1); }
.nav-btn:hover { background-color: #557290; transform: translateY(-1px); }
.nav-btn.active { background-color: #4A6785; }
.user-content { padding: 24px; flex: 1; max-width: 1000px; margin: 0 auto; width: 100%; }
.profile-grid { display: grid; grid-template-columns: 320px 1fr; gap: 24px; }
.profile-card, .orders-card { background: white; border-radius: 16px; padding: 24px; box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05); display: flex; flex-direction: column; justify-content: space-between; }
.avatar-container { display: flex; flex-direction: column; align-items: center; margin-bottom: 20px; }
.avatar-placeholder { width: 80px; height: 80px; border-radius: 50%; background: #6F8AA6; color: white; font-size: 36px; font-weight: bold; display: flex; align-items: center; justify-content: center; margin-bottom: 12px; }
.avatar-container h2 { margin: 0 0 6px 0; font-size: 20px; color: #1a3a6b; }
.role-badge { font-size: 12px; background: #eef2f6; color: #4A6785; padding: 4px 12px; border-radius: 12px; font-weight: 600; }
.info-list { display: flex; flex-direction: column; gap: 12px; border-top: 1px solid #eee; padding-top: 16px; margin-bottom: 24px; }
.info-item { display: flex; justify-content: space-between; font-size: 14px; }
.info-label { color: #777; }
.info-value { font-weight: 600; color: #333; }

/* Стили формы редактирования */
.edit-form-list { display: flex; flex-direction: column; gap: 10px; border-top: 1px solid #eee; padding-top: 16px; margin-bottom: 20px; }
.form-group { display: flex; flex-direction: column; gap: 4px; }
.form-group label { font-size: 12px; font-weight: 600; color: #555; }
.form-input { padding: 8px 10px; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 13px; outline: none; }
.form-input:focus { border-color: #6F8AA6; }
.form-actions { display: flex; gap: 8px; margin-top: 6px; }
.save-btn, .edit-btn { width: 100%; padding: 10px; border: none; background: #6F8AA6; color: white; border-radius: 8px; font-weight: 600; cursor: pointer; transition: background 0.2s; }
.save-btn:hover, .edit-btn:hover { background: #557290; }
.cancel-btn { width: 100%; padding: 10px; border: 1px solid #cbd5e1; background: transparent; color: #555; border-radius: 8px; font-weight: 600; cursor: pointer; }
.cancel-btn:hover { background: #f1f5f9; }

.logout-btn { width: 100%; padding: 10px; border: 1px solid #dc3545; background: transparent; color: #dc3545; border-radius: 8px; font-weight: 600; cursor: pointer; transition: all 0.2s; margin-top: 16px; }
.logout-btn:hover { background: #dc3545; color: white; }
.orders-card h2 { margin-top: 0; color: #1a3a6b; font-size: 20px; margin-bottom: 16px; }
.status-msg { color: #777; font-size: 15px; text-align: center; margin-top: 40px; }
.orders-list { display: flex; flex-direction: column; gap: 12px; }
.order-item { border: 1px solid #e0e6ed; border-radius: 10px; padding: 14px; background: #fdfdfd; display: flex; flex-direction: column; gap: 6px; }
.order-header { display: flex; justify-content: space-between; font-weight: bold; }
.order-id { color: #1a3a6b; }
.order-status { font-size: 13px; padding: 2px 8px; border-radius: 6px; background: #e2e8f0; color: #475569; }
.order-details { display: flex; justify-content: space-between; font-size: 14px; color: #555; }
.order-price { font-weight: bold; color: #2b6cb0; }
@media (max-width: 768px) { .profile-grid { grid-template-columns: 1fr; } }
</style>
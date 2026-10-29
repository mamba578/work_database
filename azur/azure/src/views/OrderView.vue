<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { useCartStore } from '@/stores/cart'
import {api} from '@/api/axios'

const router = useRouter()
const auth = useAuthStore()
const cart = useCartStore()

const address = ref('')
const paymentMethod = ref('card')
const comment = ref('')
const loading = ref(false)
const error = ref('')

const isFormValid = computed(() => {
  return address.value.trim().length > 5 && cart.items.length > 0
})

async function submitOrder() {
  if (!isFormValid.value) {
    alert('Пожалуйста, укажите полный адрес доставки.')
    return
  }

  if (!auth.isAuthenticated) {
    alert('Необходимо войти в систему для оформления заказа')
    router.push('/login')
    return
  }

  loading.value = true
  error.value = ''

  try {
    const orderPayload = {
      idUser: auth.user?.idUser || auth.user?.id,
      address: address.value,
      paymentMethod: paymentMethod.value,
      comment: comment.value,
      totalPrice: cart.totalPrice,
      items: cart.items.map(item => ({
        idProduct: item.idProduct,
        quantity: 1,
        price: item.price
      }))
    }

    await api.post('/orders', orderPayload)

    alert('Ваш заказ успешно оформлен!')
    cart.clearCart()
    router.push('/orders')
  } catch (e: any) {
    console.error('Ошибка при создании заказа:', e)
    error.value = e.response?.data?.message || 'Не удалось оформить заказ. Попробуйте еще раз.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="order-wrapper">
    <!-- Фирменная шапка Azure -->
    <header class="catalog-header">
      <div class="header-top">
        <h1 class="header-title">Оформление заказа «Azure»</h1>
      </div>

      <!-- Кнопки навигации -->
      <div class="header-nav">
        <button class="nav-btn" @click="router.push('/catalog')">Главная</button>
        <button class="nav-btn" @click="router.push('/cart')">Корзина ({{ cart.totalCount }})</button>
        <button class="nav-btn" @click="router.push('/profile')">Личный кабинет</button>
      </div>
    </header>

    <!-- Основной контент оформления заказа -->
    <main class="order-content">
      <div class="order-card">
        <h2>Детали и доставка</h2>

        <div v-if="cart.items.length === 0" class="empty-state">
          <p>В корзине нет товаров для оформления.</p>
          <button class="btn-primary" @click="router.push('/catalog')">Вернуться в каталог</button>
        </div>

        <form v-else @submit.prevent="submitOrder" class="order-form">
          <!-- Ошибка -->
          <div v-if="error" class="error-msg">{{ error }}</div>

          <!-- Покупатель -->
          <div class="form-section">
            <h3>Данные покупателя</h3>
            <div class="info-row">
              <span class="label">Логин:</span>
              <span class="value">{{ auth.user?.login || 'Гость' }}</span>
            </div>
            <div class="info-row" v-if="auth.user?.email">
              <span class="label">Email:</span>
              <span class="value">{{ auth.user?.email }}</span>
            </div>
            <div class="info-row" v-if="auth.user?.phone">
              <span class="label">Телефон:</span>
              <span class="value">{{ auth.user?.phone }}</span>
            </div>
          </div>

          <!-- Адрес доставки -->
          <div class="form-section">
            <h3>Доставка</h3>
            <div class="form-group">
              <label for="address">Адрес доставки *</label>
              <input
                id="address"
                v-model.trim="address"
                type="text"
                placeholder="г. Москва, ул. Пушкина, д. 10, кв. 5"
                required
              />
            </div>
            <div class="form-group">
              <label for="comment">Комментарий к заказу</label>
              <textarea
                id="comment"
                v-model.trim="comment"
                placeholder="Пожелания к доставке или упаковке..."
                rows="3"
              ></textarea>
            </div>
          </div>

          <!-- Оплата -->
          <div class="form-section">
            <h3>Способ оплаты</h3>
            <div class="radio-group">
              <label class="radio-label">
                <input type="radio" v-model="paymentMethod" value="card" />
                <span>Банковской картой онлайн</span>
              </label>
              <label class="radio-label">
                <input type="radio" v-model="paymentMethod" value="cash" />
                <span>Оплата при получении</span>
              </label>
            </div>
          </div>

          <!-- Итоговый состав -->
          <div class="form-section">
            <h3>Состав заказа</h3>
            <div class="items-summary">
              <div v-for="item in cart.items" :key="item.idProduct" class="summary-item">
                <span>{{ item.title }}</span>
                <span class="price-tag">{{ item.price }} ₽</span>
              </div>
            </div>
            <div class="total-row">
              <span>Итого к оплате:</span>
              <span class="total-price">{{ cart.totalPrice }} ₽</span>
            </div>
          </div>

          <!-- Кнопки действий -->
          <div class="form-actions">
            <button type="button" class="btn-secondary" @click="router.push('/cart')">
              ← Назад в корзину
            </button>
            <button type="submit" class="btn-primary" :disabled="loading || !isFormValid">
              {{ loading ? 'Оформление...' : 'Подтвердить заказ' }}
            </button>
          </div>
        </form>
      </div>
    </main>
  </div>
</template>

<style scoped>
/* Полноэкранный фоновый контейнер */
.order-wrapper {
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

/* Контент карточки заказа */
.order-content {
  padding: 24px;
  flex: 1;
  max-width: 750px;
  margin: 0 auto;
  width: 100%;
}

.order-card {
  background: white;
  border-radius: 16px;
  padding: 28px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.05);
}

.order-card h2 {
  margin-top: 0;
  color: #1a3a6b;
  font-size: 22px;
  margin-bottom: 20px;
  border-bottom: 2px solid #eef2f6;
  padding-bottom: 12px;
}

.empty-state {
  text-align: center;
  padding: 30px 0;
  color: #666;
}

.order-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-section {
  border: 1px solid #e2e8f0;
  border-radius: 12px;
  padding: 16px 20px;
  background: #fafbfc;
}

.form-section h3 {
  margin: 0 0 12px 0;
  font-size: 16px;
  color: #1a3a6b;
}

.info-row {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
  margin-bottom: 6px;
}

.info-row .label { color: #666; }
.info-row .value { font-weight: 600; color: #333; }

.form-group {
  display: flex;
  flex-direction: column;
  gap: 6px;
  margin-bottom: 12px;
}

.form-group:last-child { margin-bottom: 0; }

.form-group label {
  font-size: 13px;
  font-weight: 600;
  color: #555;
}

.form-group input,
.form-group textarea {
  padding: 10px 12px;
  border: 1px solid #ccc;
  border-radius: 8px;
  font-size: 14px;
  outline: none;
  font-family: inherit;
}

.form-group input:focus,
.form-group textarea:focus {
  border-color: #6F8AA6;
}

.radio-group {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.radio-label {
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 14px;
  cursor: pointer;
}

.items-summary {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 12px;
}

.summary-item {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
  color: #444;
}

.price-tag {
  font-weight: 600;
  color: #1a3a6b;
}

.total-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-top: 1px solid #cbd5e1;
  padding-top: 10px;
  font-size: 17px;
  font-weight: bold;
  color: #1a3a6b;
}

.total-price {
  font-size: 20px;
  color: #2b6cb0;
}

.form-actions {
  display: flex;
  justify-content: space-between;
  gap: 16px;
  margin-top: 8px;
}

.btn-primary {
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

.btn-primary:hover:not(:disabled) {
  background: #357ec6;
}

.btn-primary:disabled {
  background: #bdbdbd;
  cursor: not-allowed;
}

.btn-secondary {
  padding: 12px 20px;
  background: transparent;
  border: 1px solid #ccc;
  color: #555;
  border-radius: 8px;
  font-size: 14px;
  cursor: pointer;
}

.btn-secondary:hover {
  background: #f5f5f5;
  color: #333;
}

.error-msg {
  padding: 10px;
  background: #fee2e2;
  color: #dc2626;
  border-radius: 8px;
  font-size: 14px;
  text-align: center;
}
</style>
<script setup lang="ts">
import type { Product } from '@/types/product'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import { cartApi } from '@/api/cart'
import { useCartStore } from '@/stores/cart' // 1. Импортируем стор корзины

const props = defineProps<{ product: Product }>()
const auth = useAuthStore()
const router = useRouter()
const cartStore = useCartStore() // 2. Инициализируем стор

const isDeleted = props.product.status === 'Удален'
const stock = props.product.quantity ?? 0
const isOutOfStock = stock <= 0

function openDetail(e: MouseEvent) {
  if ((e.target as HTMLElement).tagName === 'BUTTON') return
  
  // Теперь id гарантированно есть
  router.push({
    path: `/product/${props.product.id}`,
    state: { product: {...props.product} }
  })
}

async function addToCart() {
  if (!auth.isAuthenticated) {
    alert('Сначала войдите в аккаунт')
    router.push('/login')
    return
  }
  
  try {
    const userId = auth.user?.id; // Убедитесь, что auth.user тоже имеет правильный интерфейс
    if (!userId) {
      alert('Ошибка: ID пользователя не найден')
      return
    }

    const activeOrder: any = await cartApi.getActiveOrder(userId)
    const orderId = activeOrder?.idOrder ?? activeOrder?.id

    if (!orderId) {
      alert('Не удалось получить активную корзину с сервера.')
      return
    }

    // Обращаемся к API корзины.
    // Если ваша корзина в БД ожидает именно idProduct, передаем его так:
    await cartApi.addItem({
      idOrder: orderId,
      idProduct: props.product.id, 
      quantity: 1,
      price: props.product.price
    })
    await cartStore.loadCartFromServer()
    alert('Товар успешно добавлен в корзину!')
  } catch (e: any) {
    console.error('Ошибка добавления в корзину:', e)
    alert(e.response?.data?.message || 'Не удалось добавить товар в корзину')
  }
}
</script>

<template>
  <div class="card" :class="{ deleted: isDeleted }" @click="openDetail">
    <!-- Если фото нет, можно вывести заглушку -->
    <img v-if="product.photo" :src="product.photo" alt="Фото товара" />
    <div v-else class="no-photo">Нет фото</div>

    <h3>{{ product.title }}</h3>
    <p class="price">{{ product.price }} ₽</p>
    <p class="status">{{ product.status }}</p>

    <button v-if="isDeleted" disabled class="btn-disabled">Удалён</button>
    <button v-else-if="isOutOfStock" disabled>Нет в наличии</button>
    <button v-else @click.stop="addToCart">В корзину</button>
  </div>
</template>

<style scoped>
.card {
  border: 1px solid #ddd;
  border-radius: 12px; /* Немного закругленные углы */
  padding: 12px;
  background: white;
  cursor: pointer;
  transition: 0.2s;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  
  /* Делает карточку строгим квадратом вне зависимости от ширины экрана */
  aspect-ratio: 1 / 1; 
  overflow: hidden; /* Скрывает контент, если он случайно выйдет за границы */
}

.card:hover { 
  transform: translateY(-3px); 
  box-shadow: 0 4px 12px rgba(0,0,0,0.1); 
}

.card.deleted { 
  opacity: 0.6; 
}

/* Картинка теперь будет занимать примерно половину квадрата */
.card img, .no-photo { 
  width: 100%; 
  height: 45%; 
  object-fit: cover; 
  border-radius: 6px; 
}

.no-photo { 
  background: #eee; 
  display: flex; 
  align-items: center; 
  justify-content: center; 
  color: #999; 
}

/* Ограничиваем текст, чтобы длинные названия не ломали квадрат */
h3 {
  font-size: 16px;
  margin: 8px 0 4px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.price { 
  font-weight: bold; 
  color: #0066cc; 
  margin: 0; 
}

.status { 
  font-size: 12px; 
  color: #666; 
  margin: 4px 0;
}

button { 
  width: 100%; 
  margin-top: auto; /* Прижимает кнопку к низу */
  padding: 8px; 
  border: none; 
  border-radius: 6px; 
  background: #6F8AA6; 
  color: white; 
  cursor: pointer; 
  transition: background 0.2s; 
}

button:hover:not(:disabled) { background: #557290; }
button:disabled { background: #bdbdbd; cursor: not-allowed; }
.btn-disabled { background: #bdbdbd; }
</style>
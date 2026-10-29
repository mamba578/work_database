<script setup lang="ts">
import type { Product } from '@/types/product'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import { cartApi } from '@/api/cart'

const props = defineProps<{ product: Product }>()
const auth = useAuthStore()
const router = useRouter()

// Безопасное получение полей через any, чтобы TypeScript не ругался на отсутствие свойств в интерфейсе
const p = props.product as any

const isDeleted = p.status === 'Удален' || p.Status === 'Удален'
const stock = p.quantity ?? p.Quantity ?? p.stockQuantity ?? 0
const isOutOfStock = stock <= 0

function imageUrl(foto: string) {
  if (!foto) return ''
  // Если путь уже полный (содержит http), возвращаем как есть
  if (foto.startsWith('http')) {
    return foto
  }
  // Если это просто имя файла, подставляем корректный адрес бэкенда и папку
  return `http://localhost:5050/Images/${encodeURIComponent(foto)}`
}

function openDetail(e: MouseEvent) {
  if ((e.target as HTMLElement).tagName === 'BUTTON') return
  const productId = p.id ?? p.idProduct ?? p.idTovar ?? p.Id ?? p.IdTovar
  
  // Переходим на детальную страницу и передаем объект товара в роутер
  router.push({
    path: `/product/${productId}`,
    state: { product: p }
  })
}

async function addToCart() {
  if (!auth.isAuthenticated) {
    alert('Сначала войдите в аккаунт')
    router.push('/login')
    return
  }
  
  try {
    const userId = auth.user?.id ?? (auth.user as any)?.Id
    if (!userId) {
      alert('Ошибка: ID пользователя не найден')
      return
    }

    // 1. Получаем активный заказ (корзину)
   // 1. Получаем активный заказ (корзину) безопасно через any
const activeOrder: any = await cartApi.getActiveOrder(userId)
const orderId = activeOrder?.idOrder ?? activeOrder?.Id ?? activeOrder?.id

    if (!orderId) {
      alert('Не удалось получить активную корзину с сервера.')
      return
    }

    // 2. Берем ID товара и цену
    const productId = p.id ?? p.idProduct ?? p.Id
    const productPrice = p.price ?? p.Price

    // 3. Добавляем товар через idProduct
    await cartApi.addItem({
      idOrder: orderId,
      idProduct: productId,
      quantity: 1,
      price: productPrice
    } as any)

    alert('Товар успешно добавлен в корзину!')
  } catch (e: any) {
    console.error('Ошибка добавления в корзину:', e)
    alert(e.response?.data?.message || 'Не удалось добавить товар в корзину')
  }
}
</script>

<template>
  <div class="card" :class="{ deleted: isDeleted }" @click="openDetail">
    <img v-if="p.photo || p.foto || p.Photo || p.Foto" :src="imageUrl(p.photo || p.foto || p.Photo || p.Foto)" />
    <h3>{{ p.title || p.Title }}</h3>
    <p class="price">{{ p.price ?? p.Price }} ₽</p>
    <p class="status">{{ p.status || p.Status }}</p>

    <button v-if="isDeleted" disabled class="btn-disabled">Удалён</button>
    <button v-else-if="isOutOfStock" disabled>Нет в наличии</button>
    <button v-else @click.stop="addToCart">В корзину</button>
  </div>
</template>

<style scoped>
.card {
  border: 1px solid #ddd;
  border-radius: 10px;
  padding: 12px;
  background: white;
  cursor: pointer;
  transition: 0.2s;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}
.card:hover { transform: translateY(-3px); box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
.card.deleted { opacity: 0.6; }
.card img { width: 100%; height: 150px; object-fit: cover; border-radius: 6px; }
.price { font-weight: bold; color: #0066cc; margin: 8px 0; }
.status { font-size: 12px; color: #666; }
button { width: 100%; margin-top: 8px; padding: 8px; border: none; border-radius: 6px; background: #6F8AA6; color: white; cursor: pointer; transition: background 0.2s; }
button:hover:not(:disabled) { background: #557290; }
button:disabled { background: #bdbdbd; cursor: not-allowed; }
.btn-disabled { background: #bdbdbd; }
</style>
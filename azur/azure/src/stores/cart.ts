import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { api } from '@/api/axios'
import { useAuthStore } from '@/stores/auth'

export const useCartStore = defineStore('cart', () => {
  const items = ref<any[]>([])
  const orderId = ref<number | null>(null) // 1. ДОБАВЛЯЕМ ПЕРЕМЕННУЮ ДЛЯ ID ЗАКАЗА

  const totalCount = computed(() => items.value.length)
  
  const totalPrice = computed(() => {
    return items.value.reduce((sum, item) => {
      const p = item.price ?? item.Price ?? 0
      const q = item.quantity ?? item.Quantity ?? 1
      return sum + (p * q)
    }, 0)
  })

  async function loadCartFromServer() {
    const auth = useAuthStore()
    if (!auth.isAuthenticated) {
      items.value = []
      orderId.value = null
      return
    }

    try {
      const userId = auth.user?.id || (auth.user as any)?.Id
      if (!userId) return

      const ordersRes = await api.get(`/Order/UserOrders/${userId}`).catch(() => ({ data: [] }))
      const activeOrder = (ordersRes.data || []).find((o: any) => (o.idStatus ?? o.IdStatus) === 1)
      const foundOrderId = activeOrder?.id ?? activeOrder?.Id

      if (foundOrderId) {
        orderId.value = foundOrderId // 2. СОХРАНЯЕМ ID КОРЗИНЫ
        const itemsRes = await api.get(`/OrderItems/ByOrder/${foundOrderId}`)
        items.value = Array.isArray(itemsRes.data) ? itemsRes.data : []
      } else {
        orderId.value = null
        items.value = []
      }
    } catch (e) {
      console.error('Ошибка загрузки корзины в Store:', e)
      items.value = []
      orderId.value = null
    }
  }

  function clearCart() {
    items.value = []
    orderId.value = null
  }

  return {
    items,
    orderId, // 3. ОБЯЗАТЕЛЬНО ЭКСПОРТИРУЕМ ЕГО НАРУЖУ
    totalCount,
    totalPrice,
    loadCartFromServer,
    clearCart
  }
})
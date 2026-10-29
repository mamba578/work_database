import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Product } from '@/types/product'

export const useCartStore = defineStore('cart', () => {
  const items = ref<Product[]>(JSON.parse(localStorage.getItem('cart') || '[]'))

  const totalCount = computed(() => items.value.length)
  const totalPrice = computed(() => items.value.reduce((sum, item) => sum + item.price, 0))

  function addToCart(product: Product) {
    items.value.push(product)
    saveCart()
  }

  function removeFromCart(index: number) {
    items.value.splice(index, 1)
    saveCart()
  }

  function clearCart() {
    items.value = []
    localStorage.removeItem('cart')
  }

  function saveCart() {
    localStorage.setItem('cart', JSON.stringify(items.value))
  }

  return {
    items,
    totalCount,
    totalPrice,
    addToCart,
    removeFromCart,
    clearCart
  }
})
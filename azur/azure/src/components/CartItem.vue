<script setup lang="ts">
import type { Cartitem } from '@/types/cartitem'

const props = defineProps<{ item: Cartitem }>()
const emit = defineEmits<{
  delete: [id: number]
  update: [item: Cartitem]
}>()

function imageUrl(foto: string) {
  return `http://localhost:5234/Foto/${encodeURIComponent(foto)}`
}

function inc() {
  props.item.quantity++
  emit('update', props.item)
}
function dec() {
  if (props.item.quantity > 1) {
    props.item.quantity--
    emit('update', props.item)
  }
}
</script>

<template>
    <div class="info">
      <h3>{{ item.idProductNavigation.title }}</h3>
      <p>{{ item.idProductNavigation.price }} ₽</p>
    </div>
    <div class="qty">
      <button @click="dec">−</button>
      <span>{{ item.quantity }}</span>
      <button @click="inc">+</button>
    </div>
    <div class="total">{{ item.totalPrice }} ₽</div>
    <button class="del" @click="emit('delete', item.idItem)">×</button>

</template>

<style scoped>
.cart-item {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  border-bottom: 1px solid #eee;
}
.cart-item img { width: 60px; height: 60px; object-fit: cover; border-radius: 6px; }
.info { flex: 1; }
.qty { display: flex; gap: 6px; align-items: center; }
.qty button { width: 28px; height: 28px; padding: 0; }
.total { font-weight: bold; min-width: 80px; text-align: right; }
.del { background: #e53935; width: 32px; height: 32px; padding: 0; border-radius: 50%; }
</style>
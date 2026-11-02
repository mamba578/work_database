<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()
const currentYear = new Date().getFullYear()
</script>

<template>
  <footer class="site-footer">
    <div class="footer-container">
      <!-- О компании -->
      <div class="footer-col brand-col">
        <h2 class="brand-name">Azure</h2>
        <p class="brand-desc">
          Ваш надежный магазин качественных товаров. Мы предлагаем лучший сервис, удобную оплату и быструю доставку.
        </p>
      </div>

      <!-- Навигация -->
      <div class="footer-col">
        <h3 class="footer-title">Покупателям</h3>
        <ul class="footer-links">
          <li><a href="#" @click.prevent="router.push('/catalog')">Каталог товаров</a></li>
          <li><a href="#" @click.prevent="router.push('/delivery')">Доставка и оплата</a></li>
          <li><a href="#" @click.prevent="router.push('/returns')">Возврат товара</a></li>
          <li><a href="#" @click.prevent="router.push('/faq')">Вопросы и ответы</a></li>
        </ul>
      </div>

      <!-- Контакты -->
      <div class="footer-col">
        <h3 class="footer-title">Контакты</h3>
        <ul class="footer-contacts">
          <li>📞 <a href="tel:+78000000000">8 (800) 000-00-00</a></li>
          <li>✉️ <a href="mailto:info@azure.com">info@azure.com</a></li>
          <li>📍 г. Уфа, ул. Ветошникова, д. 95</li>
        </ul>
      </div>
    </div>

    <!-- Нижняя полоса с копирайтом -->
    <div class="footer-bottom">
      <p>&copy; {{ currentYear }} Магазин «Azure». Все права защищены.</p>
    </div>
  </footer>
</template>

<style scoped>
.site-footer {
  background-color: #1a3a6b; /* Темно-синий из вашей палитры */
  color: #f1f5f9;
  padding: 40px 0 0 0;
  margin-top: auto; /* Прижимает футер к низу экрана, если контента мало */
}

.footer-container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 24px;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 40px;
}

.footer-col {
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.brand-name {
  font-size: 24px;
  font-weight: 700;
  color: #ffffff;
  margin: 0;
}

.brand-desc {
  font-size: 14px;
  line-height: 1.6;
  color: #cbd5e1;
  margin: 0;
}

.footer-title {
  font-size: 18px;
  font-weight: 600;
  color: #ffffff;
  margin: 0;
}

.footer-links, .footer-contacts {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.footer-links a, .footer-contacts a {
  color: #cbd5e1;
  text-decoration: none;
  font-size: 14px;
  transition: color 0.2s;
}

.footer-links a:hover, .footer-contacts a:hover {
  color: #ffffff;
  text-decoration: underline;
}

.footer-bottom {
  border-top: 1px solid #334155;
  margin-top: 40px;
  padding: 20px 24px;
  text-align: center;
  font-size: 13px;
  color: #94a3b8;
}

.footer-bottom p {
  margin: 0;
}
</style>
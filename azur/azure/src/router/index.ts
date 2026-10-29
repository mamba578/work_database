import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

import WelcomeView from '../views/WelcomeView.vue'
import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import CatalogView from '../views/CatalogView.vue'
import CartView from '../views/CartView.vue'
import UserView from '../views/ProfileView.vue'
import ProductView from '../views/ProductView.vue' 

const routes: Array<RouteRecordRaw> = [
  { path: '/', name: 'Welcome', component: WelcomeView },
  { path: '/login', name: 'Login', component: LoginView },
  { path: '/register', name: 'Register', component: RegisterView },
  { path: '/catalog', name: 'Catalog', component: CatalogView },
  { 
    path: '/product/:id', 
    name: 'Product', 
    component: ProductView 
  },
  { 
    path: '/cart', 
    name: 'Cart', 
    component: CartView,
    meta: { requiresAuth: true }
  },
  { 
    path: '/profile', 
    name: 'Profile', 
    component: UserView,
    meta: { requiresAuth: true }
  },
  {
  path: '/checkout',
  name: 'checkout',
  component: () => import('@/views/CheckoutDetailsView.vue')
}
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to, from, next) => {
  const auth = useAuthStore()

  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    next('/login')
  } else {
    next()
  }
})

export default router
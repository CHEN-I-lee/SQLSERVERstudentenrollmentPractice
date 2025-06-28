import { createRouter, createWebHistory } from 'vue-router'
import StudentsView from '../views/StudentsView.vue'
import ChartView from '../views/ChartView.vue'
// src/router/index.js
import CourseView from '../views/CourseView.vue'

const routes = [
  { path: '/', component: StudentsView },
  { path: '/charts', component: ChartView },
  { path: '/courses', component: CourseView } // ✅ 課程管理
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router

<!-- src/views/StudentsView.vue -->
<template>
  <div class="container mt-4">
    <h2>學生管理</h2>
    <Students />
  </div>
</template>

<script>
import Students from '../components/Students.vue'

export default {
  components: { Students }
}
</script>

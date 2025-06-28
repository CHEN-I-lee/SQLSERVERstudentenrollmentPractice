<!-- src/views/CourseView.vue -->
<template>
  <div class="container mt-4">
    <h3>📚 課程管理</h3>

    <!-- 新增課程 -->
    <form @submit.prevent="createCourse" class="d-flex gap-2 mb-4">
      <input v-model="newCourseName" class="form-control" placeholder="新增課程名稱" required />
      <button class="btn btn-primary">新增</button>
    </form>

    <!-- 課程列表 -->
    <table class="table table-bordered">
      <thead>
        <tr>
          <th>課程 ID</th>
          <th>課程名稱</th>
          <th>操作</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="course in courseList" :key="course.courseId">
          <td>{{ course.courseId }}</td>
          <td>
            <input v-model="course.courseName" class="form-control" />
          </td>
          <td>
            <button class="btn btn-success btn-sm me-2" @click="updateCourse(course)">修改</button>
            <button class="btn btn-danger btn-sm" @click="deleteCourse(course.courseId)">刪除</button>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
import axios from 'axios'
import { ref, onMounted } from 'vue'

export default {
  setup () {
    const newCourseName = ref('')
    const courseList = ref([])

    const fetchCourses = async () => {
      const res = await axios.get('/api/courses')
      courseList.value = res.data
    }

    const createCourse = async () => {
      try {
        console.log('送出課程名稱：', newCourseName.value)

        await axios.post('/api/courses', {
          courseName: newCourseName.value
        })

        newCourseName.value = ''
        fetchCourses()
      } catch (err) {
        console.error('新增課程失敗', err.response?.data || err.message)
        alert('新增失敗：' + (err.response?.data || '未知錯誤'))
      }
    }

    const updateCourse = async (course) => {
      try {
        await axios.put(`/api/courses/${course.courseId}`, {
          courseName: course.courseName
        })
        await fetchCourses()
        alert('課程更新成功 ✅')
      } catch (err) {
        console.error('更新課程失敗 ❌', err.response?.data || err.message)
        alert('更新失敗：' + (err.response?.data?.title || '請確認資料格式'))
      }
    }

    const deleteCourse = async (id) => {
      if (confirm('確定要刪除這門課程？')) {
        await axios.delete(`/api/courses/${id}`)
        fetchCourses()
      }
    }

    onMounted(fetchCourses)
    return {
      newCourseName,
      courseList,
      createCourse,
      updateCourse,
      deleteCourse
    }
  }
}
</script>

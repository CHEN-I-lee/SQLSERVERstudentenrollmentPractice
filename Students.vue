<template>
  <div class="container py-4">
  <h2>學生成績管理</h2>
  <div class="row g-3 mb-3">
    <div class="col-md-4">
      <input v-model="searchKeyword" class="form-control" placeholder="學生名稱關鍵字" />
    </div>
    <div class="text-end">
      <button class="btn btn-outline-secondary" @click="clearFilters">
        🔄 清除查詢條件
      </button>
    </div>
  </div>
    <!-- 🔘 新增產品按鈕 -->
    <div class="text-end mb-3">
      <button class="btn btn-success" @click="openCreateModal">➕ 新增學生</button>
    </div>

    <!-- 📦 新增學生成績 Modal -->
    <div class="modal fade" id="createModal" tabindex="-1">
    <div class="modal-dialog">
      <div class="modal-content">
        <form @submit.prevent="createNewStudent">
          <div class="modal-header">
            <h5 class="modal-title">新增學生</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body">
            <div class="mb-3">
              <label class="form-label">學生名稱</label>
              <input v-model="newStudent.name" class="form-control" required />
            </div>
            <div class="mb-3">
              <label class="form-label">課程</label>
              <select v-model.number="newStudent.courseId" class="form-select" required>
                <option disabled value="">請選擇課程</option>
                <option v-for="course in courseList" :key="course.courseId" :value="course.courseId">
                  {{ course.courseName }}
                </option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">成績</label>
              <input v-model.number="newStudent.grade" type="number" class="form-control" required />
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" data-bs-dismiss="modal">取消</button>
            <button class="btn btn-primary" type="submit">新增</button>
          </div>
        </form>
      </div>
    </div>
  </div>
  <table class="table table-striped table-hover align-middle">
    <thead class="table-dark">
      <tr>
        <th>名稱</th>
        <th>課程</th>
        <th>成績</th>
        <th class="text-end">操作</th>
      </tr>
    </thead>
    <tbody>
    <template v-for="p in paginatedStudents" :key="p.studentId">
        <div v-if="students.length === 0" class="alert alert-warning">
          🚫 尚無學生資料，請新增或確認資料是否成功取得
        </div>
        <!-- 有選課 -->
        <tr v-for="(enroll, index) in p.enrollments" :key="`${p.studentId}-${index}`">
        <td v-if="index === 0" :rowspan="p.enrollments.length">{{ p.name }}</td>
        <td>{{ enroll.courseName }}</td>
        <td>{{ enroll.grade }}</td>
        <td v-if="index === 0" :rowspan="p.enrollments.length" class="text-end">
            <button class="btn btn-sm btn-primary me-2" @click="openEditModal(enroll,p)">編輯</button>
            <button class="btn btn-sm btn-danger" @click="deleteStudent(p.studentId)">刪除</button>
        </td>
        </tr>

        <!-- 沒有選課 -->
        <tr v-if="p.enrollments.length === 0">
        <td>{{ p.name }}</td>
        <td colspan="2">尚未選課</td>
        <td class="text-end">
            <button class="btn btn-sm btn-primary me-2" @click="openEditModal(p)">編輯</button>
            <button class="btn btn-sm btn-danger" @click="deleteStudent(p.studentId)">刪除</button>
        </td>
        </tr>
    </template>
    </tbody>
  </table>

  <!-- ✏️ 編輯產品 Modal -->
  <div class="modal fade" id="editModal" tabindex="-1">
  <div class="modal-dialog">
    <div class="modal-content">
      <form @submit.prevent="saveEdit">
        <div class="modal-header">
          <h5 class="modal-title">編輯學生</h5>
          <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
        </div>

        <div class="modal-body">
          <!-- 學生名稱 -->
          <div class="mb-3">
            <label class="form-label">名稱</label>
            <input v-model="editStudent.name" class="form-control" readonly />
          </div>

          <!-- 下拉選擇學生已修過的課程 -->
          <div class="mb-3">
            <label class="form-label">課程</label>
            <select v-model.number="editStudent.courseId" class="form-select" required @change="onCourseChange">
              <option disabled value="">請選擇課程</option>
              <option v-for="enroll in editStudent.enrollments" :key="enroll.enrollmentId" :value="enroll.courseId">
                {{ enroll.courseName }}
              </option>
            </select>
          </div>

          <!-- 分數輸入 -->
          <div class="mb-3">
            <label class="form-label">成績</label>
            <input
              v-model="editStudent.grade"
              type="text"
              inputmode="numeric"
              class="form-control"
              required
              placeholder="請輸入分數"
            />
          </div>
        </div>

        <div class="modal-footer">
          <button class="btn btn-secondary" data-bs-dismiss="modal">取消</button>
          <button class="btn btn-success" type="submit">儲存變更</button>
        </div>
      </form>
    </div>
  </div>
</div>
</div>
<nav>
  <ul class="pagination justify-content-center">
    <li class="page-item" :class="{ disabled: currentPage === 1 }">
      <button class="page-link" @click="currentPage--" :disabled="currentPage === 1">上一頁</button>
    </li>

    <li
      class="page-item"
      v-for="n in totalPages"
      :key="n"
      :class="{ active: currentPage === n }"
    >
      <button class="page-link" @click="currentPage = n">{{ n }}</button>
    </li>

    <li class="page-item" :class="{ disabled: currentPage === totalPages }">
      <button class="page-link" @click="currentPage++" :disabled="currentPage === totalPages">下一頁</button>
    </li>
  </ul>
</nav>
</template>

<script>
import { Modal } from 'bootstrap'
import axios from 'axios'
import { createStudent, getStudents, updateEnrollment, deleteStudent } from '../services/studentService'

export default {
  name: 'ProductTable',
  data () {
    return {
      students: [],
      newStudent: {
        name: '',
        courseId: null,
        grade: null
      },
      courseList: [], // 從後端載入課程清單
      searchKeyword: '',
      currentPage: 1,
      pageSize: 15,
      // newStudent: { name: '', course_name: 0, grade: 0 },
      createModal: null,
      editStudent: { enrollmentId: null, name: '', courseId: 0, grade: 0, enrollments: [] },
      editModal: null // ← 儲存 Modal 實例
    }
  },
  mounted () {
    this.$nextTick(async () => {
      await this.fetchStudents()
      await this.fetchCourseList()
      // const res = await this.fetchStudents()
      // this.students = res.data

      // ✅ 等 DOM 載入後再初始化 Modal（分開判斷）
      const modalEl = document.getElementById('editModal')
      const modalElcreate = document.getElementById('createModal')

      if (modalEl) {
        this.editModal = new Modal(modalEl)
      } else {
        console.warn('找不到 editModal 元素，請確認 HTML 中是否有 id="editModal"')
      }

      if (modalElcreate) {
        this.createModal = new Modal(modalElcreate)
      } else {
        console.warn('找不到 createModal 元素，請確認 HTML 中是否有 id="createModal"')
      }
    })
  },
  computed: {
    // 搜尋過濾
    filteredStudents () {
      const list = Array.isArray(this.students) ? this.students : []
      const keyword = this.searchKeyword?.toLowerCase()?.trim()
      if (!keyword) return list

      return list.filter(s =>
        s.name?.toLowerCase()?.includes(keyword)
      )
    },

    // 分頁後的學生資料
    paginatedStudents () {
      const start = (this.currentPage - 1) * this.pageSize
      return this.filteredStudents.slice(start, start + this.pageSize)
    },

    // 額外選課資料（每位學生的第 2 筆之後）
    filteredEnrollments () {
      return this.paginatedStudents.flatMap(p =>
        Array.isArray(p.enrollments) && p.enrollments.length > 1
          ? p.enrollments.slice(1).map((e, i) => ({
            ...e,
            studentId: p.student_id,
            index: i
          }))
          : []
      )
    },
    // 分頁總數
    totalPages () {
      return Math.ceil((this.filteredStudents?.length || 0) / this.pageSize)
    }
  },
  methods: {
    openCreateModal () {
      this.newStudent = { name: '', courseName: 0, grade: 0 }
      this.createModal?.show()
    },
    onCourseChange () {
      const match = this.editStudent.enrollments.find(
        e => e.courseId === this.editStudent.courseId
      )
      if (match) {
        this.editStudent.grade = match.grade
        this.editStudent.enrollmentId = match.enrollmentId
      } else {
        this.editStudent.grade = ''
        this.editStudent.enrollmentId = null
      }
    },
    async fetchStudents () {
      try {
        const res = await getStudents()
        this.students = res
        console.log('抓回的資料', res)
        console.log('students:', this.students)
        console.log('filteredStudents:', this.filteredStudents)
        console.log('paginatedStudents:', this.paginatedStudents)
        // return res.data
      } catch (err) {
        console.error('取得學生資料失敗', err.response?.data || err.message)
      }
    },
    async createNewStudent () {
      try {
        const res = await axios.get('/api/students/exists', {
          params: { name: this.newStudent.name }
        })
        const existingStudent = res.data

        const payload = {
          name: this.newStudent.name,
          enrollments: [
            {
              courseId: this.newStudent.courseId,
              grade: this.newStudent.grade
            }
          ]
        }

        console.log('[createNewStudent] payload 準備送出：', JSON.stringify(payload, null, 2))

        if (existingStudent) {
          // 已存在學生 → 新增選課資料
          await axios.post(`/api/students/${existingStudent.studentId}/enroll`, payload.enrollments[0])
        } else {
          // 新學生 → 包含選課資料一起送出
          await createStudent(payload)
        }

        // 收尾與 UI 更新
        await this.fetchStudents()
        this.createModal?.hide()
        this.newStudent = {
          name: '',
          courseId: null,
          grade: null
        }
        this.$toast?.success?.('學生與選課資料已新增')
      } catch (err) {
        console.error('新增學生失敗 ❌', err.response?.data || err.message)
        this.$toast?.error?.('新增失敗，請確認資料是否重複或格式錯誤')
      }
    },
    openEditModal (enroll, student) {
      this.fetchCourseList() // 確保課程選單有資料

      const enrollments = Array.isArray(student.enrollments) ? student.enrollments : []

      this.editStudent = {
        name: student.name,
        studentId: student.studentId,
        courseId: '',
        grade: '',
        enrollmentId: null,
        enrollments: enrollments.map(e => ({
          enrollmentId: e.enrollmentId,
          courseId: e.courseId,
          courseName: e.courseName,
          grade: e.grade
        }))
      }

      // ✅ 如果有指定 enroll（點特定課程進來），就使用它
      if (enroll) {
        this.editStudent.courseId = enroll.courseId
        this.editStudent.grade = enroll.grade
        this.editStudent.enrollmentId = enroll.enrollmentId
      } else if (this.editStudent.enrollments.length > 0) {
        // ✅ 否則就預設選第一筆課程
        const first = this.editStudent.enrollments[0]
        this.editStudent.courseId = first.courseId
        this.editStudent.grade = first.grade
        this.editStudent.enrollmentId = first.enrollmentId
      }

      this.editModal?.show()
    },
    async fetchCourseList () {
      try {
        const res = await axios.get('/api/courses')
        this.courseList = res.data
        console.log('課程清單：', this.courseList)
      } catch (err) {
        console.error('載入課程失敗', err)
      }
    },
    async saveEdit () {
      try {
        console.log('📌 editStudent before儲存:', this.editStudent)

        const { enrollmentId, studentId, originalCourseId, courseId, grade } = this.editStudent

        if (!enrollmentId) {
          alert('找不到選課 ID，請重新選擇')
          return
        }

        const numericGrade = Number(grade)
        if (isNaN(numericGrade) || numericGrade < 0 || numericGrade > 100) {
          alert('請輸入 0 到 100 之間的成績')
          return
        }

        // 🔐 若使用者更改了課程，確認是否重複選修
        if (courseId !== originalCourseId) {
          const student = this.students.find(s => s.studentId === studentId)
          // const alreadyChosen = student?.enrollments.some(e => e.courseId === courseId)
          const alreadyChosen = student?.enrollments.some(e =>
            e.courseId === courseId && e.enrollmentId !== enrollmentId
          )
          if (alreadyChosen) {
            alert('該學生已選修此課程，請選擇其他課程')
            return
          }
        }

        console.log('送出編輯資料：', {
          enrollmentId,
          courseId,
          grade: numericGrade
        })

        await updateEnrollment(enrollmentId, {
          courseId: Number(courseId),
          grade: Number(grade)
        })

        // const res = await getStudents()
        await this.fetchStudents()
        // this.students = res.data
        this.editModal?.hide()
        this.$toast?.success?.('成績已成功更新')
      } catch (err) {
        console.error('更新學生失敗', err.response?.data || err.message)
        const errorMsg = err.response?.data?.title || err.message || '請確認資料格式'
        alert(`儲存失敗：${errorMsg}`)
      }
    },
    async deleteStudent (id) {
      if (confirm('確定要刪除嗎？')) {
        console.log('🧨 刪除學生 ID：', id, typeof id)
        await deleteStudent(id)
        // const res = await getStudents()
        // this.students = res.data
      }
      try {
        await this.fetchStudents() // ✅ 關鍵！重新載入學生列表
        this.$toast?.success?.('學生已刪除')
      } catch (err) {
        console.error('刪除失敗', err.response?.data || err.message)
        this.$toast?.error?.('刪除失敗')
      }
    },
    clearFilters () {
      this.searchKeyword = ''
      this.currentPage = 1
    }
  }
}
</script>

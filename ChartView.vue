<!-- src/views/ChartView.vue -->
<template>
  <div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h3>📊 各課程平均成績</h3>
      <select v-model="chartType" class="form-select w-auto">
        <option value="bar">柱狀圖</option>
        <option value="pie">圓餅圖</option>
        <option value="line">折線圖</option>
      </select>
    </div>
    <canvas id="courseChart" height="120"></canvas>
  </div>
</template>

<script>
import { ref, onMounted, onBeforeUnmount, watch } from 'vue'
import Chart from 'chart.js/auto'
import axios from 'axios'

export default {
  setup () {
    const chartType = ref('bar')
    const chartInstance = ref(null)

    // 資料處理函式
    const renderChart = async () => {
      const res = await axios.get('/api/students')
      const students = res.data

      // 建立課程 → [所有成績] 的 Map
      const courseMap = {}
      students.forEach(s => {
        s.enrollments.forEach(e => {
          const name = e.courseName
          if (!courseMap[name]) courseMap[name] = []
          courseMap[name].push(e.grade)
        })
      })

      // 每門課的平均成績
      const labels = Object.keys(courseMap)
      const data = labels.map(name => {
        const scores = courseMap[name]
        return Math.round(scores.reduce((a, b) => a + b, 0) / scores.length)
      })

      // 銷毀舊圖表
      if (chartInstance.value) chartInstance.value.destroy()

      const ctx = document.getElementById('courseChart').getContext('2d')
      chartInstance.value = new Chart(ctx, {
        type: chartType.value,
        data: {
          labels,
          datasets: [{
            label: '平均成績',
            data,
            backgroundColor: [
              '#0d6efd', '#6610f2', '#198754', '#ffc107', '#dc3545'
            ]
          }]
        },
        options: {
          responsive: true,
          layout: chartType.value === 'pie'
            ? {
                padding: {
                  top: 10,
                  bottom: 10,
                  left: 60,
                  right: 60
                }
              }
            : {},
          plugins: {
            title: {
              display: true,
              text: '每門課程的平均成績'
            },
            legend: {
              display: chartType.value !== 'bar'
            }
          }
        }
      })
    }

    onMounted(renderChart)
    onBeforeUnmount(() => {
      if (chartInstance.value) chartInstance.value.destroy()
    })

    watch(chartType, () => {
      renderChart()
    })

    return { chartType }
  }
}
</script>

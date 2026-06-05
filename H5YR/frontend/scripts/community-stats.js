import {
	BarController,
	BarElement,
	CategoryScale,
	Chart,
	Filler,
	Legend,
	LinearScale,
	LineController,
	LineElement,
	PointElement,
	Tooltip,
} from 'chart.js'

Chart.register(
	LineController,
	BarController,
	CategoryScale,
	LinearScale,
	PointElement,
	LineElement,
	BarElement,
	Tooltip,
	Legend,
	Filler,
)

const PURPLE = '#6b2fa0'
const PURPLE_LIGHT = 'rgba(107, 47, 160, 0.2)'

export function initCommunityStats() {
	const summaryEl = document.getElementById('js-stats-summary')
	const activityEl = document.getElementById('js-activity-chart')
	const contributorsEl = document.getElementById('js-contributors-chart')

	if (!summaryEl && !activityEl && !contributorsEl) return

	fetch('/api/communitystats')
		.then((response) => {
			if (!response.ok) throw new Error(`HTTP ${response.status}`)
			return response.json()
		})
		.then((data) => {
			console.log('Community stats loaded:', data)
			if (summaryEl) renderSummary(data)
			renderActivityChart(data.activityTimeline || [])
			renderContributorsChart(data.topContributors || [])
		})
		.catch((err) => {
			console.error('Failed to load community stats:', err)
			if (activityEl)
				activityEl.parentElement.innerHTML +=
					'<p class="stats-chart__empty">Unable to load data</p>'
			if (contributorsEl)
				contributorsEl.innerHTML =
					'<p class="stats-chart__empty">Unable to load data</p>'
		})
}

function renderSummary(data) {
	const total = data.totalH5yrs ?? data.totalH5Yrs ?? 0
	const widget = data.totalWidgetSubmissions ?? 0
	const today = data.todayCount ?? 0

	const totalEl = document.getElementById('js-stat-total')
	const widgetEl = document.getElementById('js-stat-widget')
	const todayEl = document.getElementById('js-stat-today')

	// Animate counters when they come into view
	animateCounterOnScroll(totalEl, total)
	animateCounterOnScroll(widgetEl, widget)
	animateCounterOnScroll(todayEl, today)
}

function animateCounterOnScroll(element, target) {
	const observer = new IntersectionObserver((entries) => {
		entries.forEach(entry => {
			if (entry.isIntersecting && !element.dataset.animated) {
				animateCounter(element, target)
				element.dataset.animated = 'true'
				observer.disconnect()
			}
		})
	}, { 
		threshold: 0
	})

	observer.observe(element)
}

function animateCounter(element, target) {
	const duration = 1500 // 1.5 seconds
	const startTime = performance.now()

	function update(currentTime) {
		const elapsed = currentTime - startTime
		const progress = Math.min(elapsed / duration, 1)

		// Linear progress (no easing)
		const current = Math.floor(progress * target)

		element.textContent = current.toLocaleString()

		if (progress < 1) {
			requestAnimationFrame(update)
		} else {
			element.textContent = target.toLocaleString()
		}
	}

	requestAnimationFrame(update)
}

function renderActivityChart(timeline) {
	const ctx = document.getElementById('js-activity-chart')
	if (!ctx) return
	if (!timeline.length) {
		ctx.parentElement.innerHTML +=
			'<p class="stats-chart__empty">No activity data yet</p>'
		return
	}

	new Chart(ctx, {
		type: 'bar',
		data: {
			labels: timeline.map((d) => d.label),
			datasets: [
				{
					label: 'High Fives',
					data: timeline.map((d) => d.count),
					backgroundColor: PURPLE,
					borderColor: PURPLE,
					borderWidth: 1,
					borderRadius: 4,
					maxBarThickness: 20,
				},
			],
		},
		options: {
			responsive: true,
			maintainAspectRatio: false,
			plugins: {
				legend: { display: false },
				tooltip: {
					backgroundColor: PURPLE,
					titleFont: { size: 13 },
					bodyFont: { size: 12 },
				},
			},
			scales: {
				y: {
					beginAtZero: true,
					ticks: { precision: 0 },
				},
			},
		},
	})
}

function renderContributorsChart(contributors) {
	const container = document.getElementById('js-contributors-chart')
	if (!container) return
	if (!contributors.length) {
		container.innerHTML =
			'<p class="stats-chart__empty">No contributors yet</p>'
		return
	}

	const maxCount = Math.max(...contributors.map((c) => c.count))

	container.innerHTML = contributors
		.map((c) => {
			const barWidth = (c.count / maxCount) * 100
			const profileUrl = c.profileUrl || '#'
			return `
            <div class="stats-contributor">
                <a href="${profileUrl}" target="_blank" rel="noopener" class="stats-contributor__info">
                    <img src="${c.avatarUrl}" alt="${c.displayName}" class="stats-contributor__avatar" width="24" height="24" />
                    <span class="stats-contributor__name">${c.displayName}</span>
                </a>
                <div class="stats-contributor__bar-wrap">
                    <div class="stats-contributor__bar" style="width: ${barWidth}%"></div>
                    <span class="stats-contributor__count">${c.count}</span>
                </div>
            </div>`
		})
		.join('')
}

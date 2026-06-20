const featuredProjects = document.querySelectorAll(".dashboard-featured-project");

if (featuredProjects.length > 1) {
    let activeIndex = 0;

    setInterval(function () {
        featuredProjects[activeIndex].classList.remove("is-active");
        activeIndex = (activeIndex + 1) % featuredProjects.length;
        featuredProjects[activeIndex].classList.add("is-active");
    }, 5000);
}

const languageChart = document.getElementById("languageChart");

if (languageChart && window.Chart) {
    const labels = JSON.parse(languageChart.dataset.labels || "[]");
    const counts = JSON.parse(languageChart.dataset.counts || "[]");

    if (labels.length > 0) {
        new Chart(languageChart, {
            type: "doughnut",
            data: {
                labels: labels,
                datasets: [{
                    data: counts,
                    backgroundColor: [
                        "#0d6efd",
                        "#14b8a6",
                        "#f59e0b",
                        "#ef4444",
                        "#8b5cf6",
                        "#64748b"
                    ],
                    borderWidth: 0
                }]
            },
            options: {
                plugins: {
                    legend: {
                        position: "bottom"
                    }
                },
                cutout: "62%"
            }
        });
    }
}
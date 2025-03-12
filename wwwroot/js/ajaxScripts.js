document.addEventListener("DOMContentLoaded", () => {
    async function loadStandards(topicId) {
        let standardsDiv = document.getElementById(`standards-${topicId}`);

        // Toggle visibility if already loaded
        if (standardsDiv.innerHTML.trim() !== "") {
            standardsDiv.style.display = standardsDiv.style.display === "none" ? "block" : "none";
            return;
        }

        try {
            let response = await fetch(`/Index?handler=LoadStandards&topicId=${topicId}`);
            let data = await response.text();
            standardsDiv.innerHTML = data;
            standardsDiv.style.display = "block";
        } catch (error) {
            console.error("Error loading standards:", error);
        }

        document.getElementById("topicId").value = topicId;
    };

    async function loadStandard(topicId, standardId) {
        let standardDiv = document.getElementById('standard');

        try {
            let response = await fetch(`/Index?handler=LoadStandard&topicId=${topicId}&standardId=${standardId}`);
            standardDiv.innerHTML = await response.text();
            standardDiv.style.display = "block";
        } catch (error) {
            console.error("Error loading standard:", error);
        }

        document.getElementById("standardId").value = standardId;
    };

    async function submitForm(event) {
        event.preventDefault();

        let topicId = document.getElementById("topicId").value;
        let standardId = document.getElementById("standardId").value;

        console.log(topicId, standardId);

        let formData = new FormData();
        formData.append("topicId", topicId);
        formData.append("standardId", standardId);

        console.log(formData)

        try {
            let response = await fetch("/Index?handler=Post", {
                method: "POST",
                body: formData,
            });

            console.log(response)

            if (!response.ok) throw new Error("Failed to fetch AI response");

            document.getElementById("ai-response").innerHTML = await response.text();
        } catch (error) {
            console.error("Error:", error);
            document.getElementById("ai-response").innerHTML = `<pre class="alert alert-danger">Error fetching response.</pre>`;
        }
    }

    window.loadStandards = loadStandards;
    window.loadStandard = loadStandard;
    document.querySelector("form").addEventListener("submit", submitForm);
});

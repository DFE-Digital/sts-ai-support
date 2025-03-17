document.addEventListener("DOMContentLoaded", () => {
    async function submitForm(event) {
        event.preventDefault();

        let userPrompt = document.getElementById("user-prompt").value;
        let systemPrompt = document.getElementById("system-prompt").value;

        let formData = new FormData();
        formData.append("userPrompt", userPrompt);
        formData.append("systemPrompt", systemPrompt);

        try {
            document.getElementById("ai-form").style.display = "none";
            document.getElementById("generating-spinner").style.display = "block";

            let response = await fetch("/StandardsGenerator?handler=Post", {
                method: "POST",
                body: formData,
            });

            if (!response.ok) throw new Error("Failed to fetch AI response");

            document.getElementById("ai-response").innerHTML = await response.text();
        } catch (error) {
            console.error("Error:", error);
            document.getElementById("ai-response").innerHTML = `<pre class="alert alert-danger">Error fetching response.</pre>`;
        }
        finally {
            document.getElementById("ai-form").style.display = "block";
            document.getElementById("generating-spinner").style.display = "none";
        }
    }

    document.querySelector("form").addEventListener("submit", submitForm);
});

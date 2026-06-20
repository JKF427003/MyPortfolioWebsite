let profileCropper = null;

const photoInput = document.getElementById("profilePhotoInput");
const photoEditor = document.getElementById("photoEditor");
const photoEditorImage = document.getElementById("photoEditorImage");
const croppedPhotoData = document.getElementById("croppedPhotoData");

if (photoInput && photoEditor && photoEditorImage && croppedPhotoData) {
    photoInput.addEventListener("change", function () {
        const file = photoInput.files && photoInput.files[0];

        if (!file) {
            return;
        }

        const allowedTypes = ["image/jpeg", "image/png", "image/webp"];

        if (!allowedTypes.includes(file.type)) {
            alert("Please choose a JPG, PNG, or WEBP image.");
            photoInput.value = "";
            return;
        }

        const reader = new FileReader();

        reader.onload = function (event) {
            photoEditorImage.src = event.target.result;
            photoEditor.classList.remove("d-none");

            if (profileCropper) {
                profileCropper.destroy();
            }

            profileCropper = new Cropper(photoEditorImage, {
                aspectRatio: 1,
                viewMode: 1,
                dragMode: "move",
                autoCropArea: 0.9,
                responsive: true,
                background: false,
                movable: true,
                zoomable: true,
                rotatable: false,
                scalable: false
            });
        };

        reader.readAsDataURL(file);
    });

    const form = photoInput.closest("form");

    if (form) {
        form.addEventListener("submit", function () {
            if (!profileCropper) {
                return;
            }

            const canvas = profileCropper.getCroppedCanvas({
                width: 512,
                height: 512,
                imageSmoothingEnabled: true,
                imageSmoothingQuality: "high"
            });

            croppedPhotoData.value = canvas.toDataURL("image/jpeg", 0.9);
        });
    }
}

window.addEventListener("storage", function (event) {
    if (event.key === "profile-email-verified") {
        window.location.reload();
    }
});

document.querySelectorAll("[data-copy-url]").forEach(function (button) {
    button.addEventListener("click", async function () {
        const url = button.getAttribute("data-copy-url");

        if (!url || !navigator.clipboard) {
            return;
        }

        await navigator.clipboard.writeText(url);

        const originalText = button.innerHTML;
        button.innerHTML = '<i class="bi bi-check2 me-1"></i>Copied';

        window.setTimeout(function () {
            button.innerHTML = originalText;
        }, 1800);
    });
});

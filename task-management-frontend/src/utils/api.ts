export function parseApiError(err: any): string {
    const data = err?.response?.data;

    if (data?.message) return data.message;

    // ModelState validation errors
    if (data?.errors) return Object.values(data.errors).flat().join(" ");

    return "Something went wrong. Please try again.";
}
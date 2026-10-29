export default function formatTime(createdAt: string) {
    const diff = Date.now() - new Date(createdAt).getTime();
    const minutes = Math.floor(diff / 60000);

    if (minutes < 1) return "Az önce";
    if (minutes < 60) return `${minutes} dk önce`;

    const hours = Math.floor(minutes / 60);

    if (hours < 24) return `${hours} saat önce`;

    const days = Math.floor(hours / 24);
    return `${days} gün önce`;
}
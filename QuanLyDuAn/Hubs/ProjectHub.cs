using Microsoft.AspNetCore.SignalR;

namespace QuanLyDuAn.Hubs
{
    public class ProjectHub : Hub
    {
        // Client joins a project group for real-time updates
        public async Task JoinProject(string maDuAn)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, maDuAn);
        }

        public async Task LeaveProject(string maDuAn)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, maDuAn);
        }

        // Notify all clients in a project when a task status changes (Kanban drag-drop)
        public async Task TaskStatusChanged(string maDuAn, string maCongViec, string newStatus)
        {
            await Clients.OthersInGroup(maDuAn).SendAsync("OnTaskStatusChanged", maCongViec, newStatus);
        }

        // Notify all clients when a new comment is added
        public async Task NewComment(string maDuAn, string maCongViec, string hoTen, string noiDung, string thoiGian)
        {
            await Clients.OthersInGroup(maDuAn).SendAsync("OnNewComment", maCongViec, hoTen, noiDung, thoiGian);
        }
    }
}

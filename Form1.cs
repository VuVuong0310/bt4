using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace lab01
{
    public partial class frmSinhvien : Form
    {
        public frmSinhvien()
        {
            InitializeComponent();
        }

     

        private void frmSinhvien_Load(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<Faculty> listFalcultys = context.Faculties.ToList(); //lấy các khoa
                List<Student> listStudent = context.Students.ToList(); //lấy sinh viên
                FillFalcultyCombobox(listFalcultys);
                BindGrid(listStudent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
        private void FillFalcultyCombobox(List<Faculty> listFalcultys)
        {
            this.cmbKhoa.DataSource = listFalcultys;
            this.cmbKhoa.DisplayMember = "FacultyName";
            this.cmbKhoa.ValueMember = "FacultyID";
        }
        //Hàm binding gridView từ list sinh viên
        private void BindGrid(List<Student> listStudent)
        {
            dgvSinhvien.Rows.Clear();
            foreach (var item in listStudent)
            {
                int index = dgvSinhvien.Rows.Add();
                dgvSinhvien.Rows[index].Cells[0].Value = item.StudentID;
                dgvSinhvien.Rows[index].Cells[1].Value = item.FullName;
                dgvSinhvien.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvSinhvien.Rows[index].Cells[3].Value = item.AverageScore;
                
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                // Khởi tạo context để làm việc với database

                Model1 db = new Model1();
                // Lấy danh sách tất cả sinh viên trong CSDL
                List<Student> studentList = db.Students.ToList();

                // Kiểm tra trùng mã sinh viên
                if (studentList.Any(s => s.StudentID == (txtMa.Text)))
                {
                    MessageBox.Show("Mã SV đã tồn tại. Vui lòng nhập một mã khác.",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Tạo đối tượng sinh viên mới
                var newStudent = new Student
                {
                    StudentID = txtMa.Text,
                    FullName = txtTen.Text,
                    FacultyID = int.Parse(cmbKhoa.SelectedValue.ToString()),
                    AverageScore = float.Parse(txtDiem.Text)
                };

            // Thêm sinh viên vào CSDL
            db.Students.Add(newStudent);
            db.SaveChanges();

            // Hiển thị lại danh sách sinh viên sau khi thêm
            BindGrid(db.Students.ToList());

            // Thông báo thành công
            MessageBox.Show("Thêm sinh viên thành công!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi khi thêm dữ liệu
                MessageBox.Show($"Lỗi khi thêm dữ liệu: {ex.Message}",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
}

        private void dgvSinhvien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectRow = dgvSinhvien.Rows[e.RowIndex];
                txtMa.Text = selectRow.Cells[0].Value.ToString();
                txtTen.Text = selectRow.Cells[1].Value.ToString();
                cmbKhoa.Text = selectRow.Cells[2].Value.ToString();
                txtDiem.Text = selectRow.Cells[3].Value.ToString();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // Khởi tạo context để làm việc với CSDL
                Model1 db = new Model1();

                // Lấy danh sách sinh viên từ CSDL
                List<Student> students = db.Students.ToList();

                // Tìm sinh viên cần cập nhật theo mã sinh viên
             var student = students.FirstOrDefault(s => s.StudentID== (txtMa.Text));

                if (student != null)
                {
                    // Kiểm tra trùng mã sinh viên ngoại trừ sinh viên hiện tại
                    if (students.Any(s => s.StudentID == (txtMa.Text) && s.StudentID != student.StudentID   ))
                    {
                        MessageBox.Show("Mã SV đã tồn tại. Vui lòng nhập một mã khác.",
                                        "Thông báo",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        return;
                    }

                    // Cập nhật thông tin sinh viên
                    student.FullName = txtTen.Text;

                    student.FacultyID = int.Parse(cmbKhoa.SelectedValue.ToString());
                    student.AverageScore = float.Parse(txtDiem.Text);

                    // Lưu thay đổi vào CSDL
                    db.SaveChanges();

                    // Hiển thị lại danh sách sinh viên sau khi cập nhật
                    BindGrid(db.Students.ToList());

                    // Thông báo thành công
                    MessageBox.Show("Chỉnh sửa thông tin sinh viên thành công!",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    // Thông báo khi không tìm thấy sinh viên
                    MessageBox.Show("Sinh viên không tìm thấy!",
                                    "Thông báo",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Thông báo lỗi khi xảy ra ngoại lệ
                MessageBox.Show($"Lỗi khi cập nhật dữ liệu: {ex.Message}",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                Model1 context = new Model1();
                List<Student> students = context.Students.ToList();
                var student = students.FirstOrDefault(s => s.StudentID == (txtMa.Text));
                if (student != null)
                {
                    context.Students.Remove(student);
                    context.SaveChanges();
                    BindGrid(context.Students.ToList());
                    MessageBox.Show("Sinh vien da duoc xoa thanh cong!", "Thong bao!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Sinh vien khong tim thay!", "Thong bao!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loi khi cap nhat du lieu: {ex.Message}", "Thong bao!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn thoát chương trình không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit(); // Thoát chương trình
            }


        }

        private void quanlykhoaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmKhoa frmKhoa = new frmKhoa();
            frmKhoa.Show();
            Hide();
        }

        private void timkiemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTim frmTim = new frmTim();
            frmTim.Show();
            Hide();
        }
    }
    }

